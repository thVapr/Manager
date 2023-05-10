
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
            await database.Projects.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CreateEntity(Guid id, ProjectDataModel model)
    {
        await CreateEntity(model);
        await using var database = new ManagerDbContext();

        try
        {
            var department = await database.Departments.Where(e => e.Id == id).FirstOrDefaultAsync();
            
            if (department == null) return false;

            await database.DepartmentProjects.AddAsync(
                new DepartmentProjectsDataModel
                {
                    DepartmentId = id,
                    ProjectId = model.Id,
                });
            
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
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
            await database.ProjectEmployees.AddAsync(
                new ProjectEmployeesDataModel
                {
                    ProjectId = firstId,
                    EmployeeId = secondId,
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

    public Task<IEnumerable<ProjectDataModel>?> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProjectDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var departmentId = await database.DepartmentProjects.Where(d => d.DepartmentId == id).ToListAsync();
            var entities = new List<ProjectDataModel>();

            if (entities == null) throw new ArgumentNullException(nameof(entities));

            foreach (var v in departmentId)
            {
                entities.Add(await database.Projects.Where(p => p.Id == v.ProjectId).FirstOrDefaultAsync() ?? throw new InvalidOperationException());
            }

            return entities;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Enumerable.Empty<ProjectDataModel>();
        }
    }

    public async Task<bool> UpdateEntity(ProjectDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var project = await database.Projects.FindAsync(model.Id);

            if (project == null) return false;

            project.Name = model.Name;
            project.Description = model.Description;

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
            var existingProject = await database.Projects.FindAsync(id);

            if (existingProject == null) return false;

            database.Projects.Remove(existingProject);
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