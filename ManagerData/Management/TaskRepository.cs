
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class TaskRepository : IManagementRepository<TaskDataModel>
{
    public async Task<bool> CreateEntity(TaskDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingTask = await database.Tasks.Where(m => m.Name == model.Name).FirstOrDefaultAsync();

            if (existingTask != null) return false;

            await database.Tasks.AddAsync(model);

            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> CreateEntity(Guid id, TaskDataModel model)
    {
        if (!await CreateEntity(model)) return false;
        await using var database = new ManagerDbContext();

        try
        {
            var project = database.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null) return false;

            await database.ProjectTasks.AddAsync(new ProjectTasksDataModel()
            {
                ProjectId = id,
                TaskId = model.Id,
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
            await database.EmployeeTasks.AddAsync(new EmployeeTasksDataModel()
            {
                EmployeeId = firstId,
                TaskId = secondId,
            });
            
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<TaskDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Tasks.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new TaskDataModel();
        }
        catch
        {
            return new TaskDataModel();
        }
    }

    public Task<IEnumerable<TaskDataModel>?> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TaskDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var tasksId = await database.ProjectTasks.Where(d => d.ProjectId == id).ToListAsync();
            var entities = new List<TaskDataModel>();

            if (entities == null) throw new ArgumentNullException(nameof(entities));

            foreach (var v in tasksId)
            {
                entities.Add(await database.Tasks.Where(p => p.Id == v.ProjectId).FirstOrDefaultAsync() ?? throw new InvalidOperationException());
            }

            return entities;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Enumerable.Empty<TaskDataModel>();
        }
    }

    public async Task<bool> UpdateEntity(TaskDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var task = await database.Tasks.FindAsync(model.Id);

            if (task == null) return false;

            task.Name = model.Name;
            task.Description = model.Description;
            task.EmployeeId = model.EmployeeId;
            task.Level = model.Level;

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
            var existingTask = await database.Tasks.FindAsync(id);

            if (existingTask == null) return false;

            database.Tasks.Remove(existingTask);
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