using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class PartRepository : IManagementRepository<PartDataModel>
{
    public async Task<bool> CreateEntity(PartDataModel model)
    {
        await using var database = new MainDbContext();

        try
        {
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

            if (model.Level > 0)
            {
                await database.PartLinks.AddAsync(
                    new PartLink
                    {
                        MasterId = masterPartId, SlaveId = model.Id
                    });
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
                new PartMembersDataModel()
                {
                    PartId = destinationId,
                    MemberId = sourceId,
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

            await database.PartLinks.AddAsync(
                new PartLink
                {
                    MasterId = masterId,
                    SlaveId = slaveId,
                }
            );
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
            var link = await database.PartLinks
                .Where(pl => pl.MasterId == masterId && pl.SlaveId == slaveId)
                .FirstOrDefaultAsync();

            if (link == null) return false;

            database.PartLinks.Remove(link);
            
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

    public Task<IEnumerable<PartDataModel>?> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PartDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var departments = await database.PartLinks.Where(c => c.MasterId == id).ToListAsync();
            List<PartDataModel?> result = [];
            
            if (result == null) throw new ArgumentNullException(nameof(result));

            foreach (var v in departments)
            {
                result.Add(await database.Parts.Where(d => d.Id == v.SlaveId).FirstOrDefaultAsync());
            }

            return result!;
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

    public void Dispose()
    {
    }
}