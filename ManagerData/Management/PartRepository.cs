using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class PartRepository : IManagementRepository<PartDataModel>
{
    public async Task<bool> CreateEntity(PartDataModel model)
    {
        await using var database = new ManagerDbContext();

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

    public async Task<bool> CreateEntity(Guid workspaceId, PartDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingDepartment = await database.Parts.Where(m => m.Name == model.Name).FirstOrDefaultAsync();
            var existingLink = await database.WorkspaceParts.Where(m => m.WorkspaceId == workspaceId).FirstOrDefaultAsync();

            if (existingDepartment != null && existingLink != null) return false;

            await CreateEntity(model);

            var company = await database.Workspaces.Where(e => e.Id == workspaceId).FirstOrDefaultAsync();

            if (company == null) return false;

            await database.WorkspaceParts.AddAsync(
                new WorkspacePartsDataModel
                {
                    WorkspaceId = workspaceId, PartId = model.Id
                });
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
        await using var database = new ManagerDbContext();

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
        await using var database = new ManagerDbContext();

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
        await using var database = new ManagerDbContext();

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
        await using var database = new ManagerDbContext();

        try
        {
            var departments = await database.WorkspaceParts.Where(c => c.WorkspaceId == id).ToListAsync();
            List<PartDataModel?> result = new();

            if (result == null) throw new ArgumentNullException(nameof(result));

            foreach (var v in departments)
            {
                result.Add(await database.Parts.Where(d => d.Id == v.PartId).FirstOrDefaultAsync());
            }

            return result!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Enumerable.Empty<PartDataModel>();
        }
    }

    public async Task<bool> UpdateEntity(PartDataModel model)
    {
        await using var database = new ManagerDbContext();

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
        await using var database = new ManagerDbContext();

        try
        {
            var existingCompany = await database.Workspaces.FindAsync(id);

            if (existingCompany == null) return false;

            database.Workspaces.Remove(existingCompany);
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