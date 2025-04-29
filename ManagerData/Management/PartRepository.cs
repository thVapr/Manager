using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class PartRepository : IPartRepository
{
    public async Task<bool> CreateEntity(PartDataModel model)
    {
        await using var database = new MainDbContext();

        try
        {
            var partType = await database.PartTypes.FindAsync(model.TypeId);
            if (partType is null)
            {
                if (new [] {1,2,3}.Contains(model.TypeId))
                    await SeedPartTypes();
                else
                    return false;
            }
            await database.Parts.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> CreateEntity(Guid masterPartId, PartDataModel model)
    {
        await using var database = new MainDbContext();

        try
        {
            var existingPart = await database.Parts.Where(m => m.Name == model.Name).FirstOrDefaultAsync();
            if (existingPart != null) 
                return false;

            await CreateEntity(model);

            if (masterPartId != Guid.Empty)
            {
                await LinkEntities(masterPartId, model.Id);
            }
            await database.SaveChangesAsync();

            return true;
        }
        catch(Exception ex) 
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> AddToEntity(Guid destinationId, Guid sourceId)
    {
        await using var database = new MainDbContext();

        try
        {
            await database.PartMembers.AddAsync(
                new PartMemberDataModel()
                {
                    PartId = destinationId,
                    MemberId = sourceId,
                    Privileges = 1 //TODO: Определить уровни привилегий
                }
            );

            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId)
    {
        await using var database = new MainDbContext();

        try
        {
            var link = await database.PartMembers
                .Where(de => de.PartId == destinationId && de.MemberId == sourceId)
                .FirstOrDefaultAsync();

            if (link == null) return false;

            database.PartMembers.Remove(link);
            
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> LinkEntities(Guid masterId, Guid slaveId)
    {
        await using var database = new MainDbContext();

        try
        {
            var master = await database.Parts.Where(m => m.Id == masterId).FirstOrDefaultAsync();
            var slave = await database.Parts.Where(m => m.Id == slaveId).FirstOrDefaultAsync();
            if (master is null || slave is null)
                return false;

            slave.MainPartId = master.Id;
            slave.Level = master.Level + 1;
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> UnlinkEntities(Guid masterId, Guid slaveId)
    {
        await using var database = new MainDbContext();

        try
        {
            var link = await database.Parts.FindAsync(slaveId);

            if (link == null && link!.MainPartId == masterId) return false;

            link.MainPartId = null;
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<PartDataModel> GetEntityById(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            return await database.Parts.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new PartDataModel();
        }
        catch
        {
            return new PartDataModel();
        }
    }

    public async Task<IEnumerable<PartDataModel>?> GetEntities()
    {
        await using var database = new MainDbContext();

        try
        {
            return await database.Parts.ToListAsync();
        }
        catch
        {
            return [];
        }
    }

    public async Task<IEnumerable<PartDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var parts = await database.Parts
                .Where(c => c.MainPartId == id)
                .ToListAsync();

            return parts;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }

    public async Task<bool> UpdateEntity(PartDataModel model)
    {
        await using var database = new MainDbContext();

        try
        {
            var department = await database.Parts.Where(c => c.Id == model.Id).FirstOrDefaultAsync();

            if (department == null) return false;
            
            // TODO: Нужно написать метод, принимающий множество параметров для их валидации
            if (!string.IsNullOrEmpty(model.Name))
                department.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Description))
                department.Description = model.Description;
            if (model.Level >= 0)
                department.Level = model.Level;
            if (model.MainPartId.HasValue)
                department.MainPartId = model.MainPartId == Guid.Empty ? null : model.MainPartId;
            
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> DeleteEntity(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var existingPart = await database.Parts.FindAsync(id);
            if (existingPart == null) return false;

            database.Parts.Remove(existingPart);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<List<PartDataModel>> GetLinks(Guid partId)
    {
        await using var database = new MainDbContext();

        try
        {
            return await database.Parts
                .Where(pl => pl.MainPartId == partId ||
                    pl.Parts.Any(p => p.Id == partId)).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public async Task<IEnumerable<PartMemberDataModel>> GetPartMembers(Guid partId)
    {
        await using var database = new MainDbContext();

        try
        {
            return await database.PartMembers.Where(pm => pm.PartId == partId).ToListAsync();;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public async Task<bool> SetPrivileges(Guid userId, Guid partId, int privilege)
    {
        await using var database = new MainDbContext();

        try
        {
            var partMember = await database.PartMembers
                .Where(pm => pm.PartId == partId && pm.MemberId == userId)
                .FirstOrDefaultAsync();
            if (partMember == null)
            {
                var member = await database.Members
                    .Where(m => m.Id == userId).FirstOrDefaultAsync();
                if (member == null)
                    return false;
                await AddToEntity(partId, member.Id);
            }
            partMember = await database.PartMembers
                .Where(pm => pm.PartId == partId && pm.MemberId == userId)
                .FirstOrDefaultAsync();
            if (partMember == null) return false;
            
            partMember.Privileges = privilege;
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private async Task SeedPartTypes()
    {
        await AddPartType(Constants.PartTypeConstants.Root);
        await AddPartType(Constants.PartTypeConstants.Group);
        await AddPartType(Constants.PartTypeConstants.Project);
    }

    private async Task<bool> AddPartType(string name)
    {
        await using var database = new MainDbContext();

        try
        {
            var partType = await database.PartTypes.FirstOrDefaultAsync(r => r.Name == name);

            if (partType != null) return false;

            var part = new PartType
            {
                Name = name
            };

            await database.PartTypes.AddAsync(part);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
    
    public void Dispose()
    {
    }
}