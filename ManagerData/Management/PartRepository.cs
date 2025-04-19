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

            if (existingPart != null) return false;

            await CreateEntity(model);

            if (model.Level > 0)
            {
                await database.PartLinks.AddAsync(
                    new PartLinks
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

    public async Task<bool> LinkEntities(Guid firstId, Guid secondId)
    {
        await using var database = new MainDbContext();

        try
        {
            await database.PartMembers.AddAsync(
                new PartMembersDataModel()
                {
                    PartId = firstId,
                    MemberId = secondId,
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

    public async Task<bool> UnlinkEntities(Guid firstId, Guid secondId)
    {
        await using var database = new MainDbContext();

        try
        {
            var link = await database.PartMembers
                .Where(de => de.PartId == firstId && de.MemberId == secondId)
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
            List<PartDataModel?> result = new();

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

            if (!string.IsNullOrEmpty(model.Name))
                department.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Description))
                department.Description = model.Description;
            if (model.ManagerId != null)
                department.ManagerId = model.ManagerId;

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