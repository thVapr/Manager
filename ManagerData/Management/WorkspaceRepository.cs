
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class WorkspaceRepository : IManagementRepository<WorkspaceDataModel>
{
    public async Task<bool> CreateEntity(WorkspaceDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingCompany = await database.Workspaces.Where(m => m.Name == model.Name).FirstOrDefaultAsync();

            if (existingCompany != null) return false;

            await database.Workspaces.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch(Exception ex) 
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public Task<bool> CreateEntity(Guid id, WorkspaceDataModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LinkEntities(Guid firstId, Guid secondId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UnlinkEntities(Guid firstId, Guid secondId)
    {
        throw new NotImplementedException();
    }

    public async Task<WorkspaceDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Workspaces.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new WorkspaceDataModel();
        }
        catch 
        {
            return new WorkspaceDataModel();
        }
    }

    public async Task<IEnumerable<WorkspaceDataModel>?> GetEntities()
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Workspaces.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
        
    public Task<IEnumerable<WorkspaceDataModel>?> GetEntitiesById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateEntity(WorkspaceDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var company = await database.Workspaces.FindAsync(model.Id);

            if (company == null) return false;
            
            company.Name = model.Name;
            company.Description = model.Description;

            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
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
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
    }
}