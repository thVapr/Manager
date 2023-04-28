
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class ProjectRepository : IManagementRepository<ProjectDataModel>
{
    public async Task<bool> CreateEntity(ProjectDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingProject = await database.Projects.Where(m => m.Name == model.Name).FirstOrDefaultAsync();

            if (existingProject != null) return false;

            await database.Projects.AddAsync(model);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ProjectDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Projects.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new ProjectDataModel();
        }
        catch
        {
            return new ProjectDataModel();
        }
    }

    public async Task<bool> UpdateEntity(ProjectDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var project = await database.Projects.FindAsync(model.Id);

            if (project == null) return false;

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
            var existingProject = await database.Employees.FindAsync(id);

            if (existingProject == null) return false;

            database.Employees.Remove(existingProject);
            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}