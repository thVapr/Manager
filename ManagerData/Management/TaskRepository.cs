
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class TaskRepository : ITaskRepository
{
    public async Task<bool> CreateEntity(TaskDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
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
            var project = database.Parts.FirstOrDefault(p => p.Id == id);

            if (project == null) return false;

            await database.PartTasks.AddAsync(new PartTasksDataModel()
            {
                PartId = id,
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
            await database.MemberTasks.AddAsync(new MemberTasksDataModel()
            {
                MemberId = firstId,
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

    public async Task<bool> UnlinkEntities(Guid firstId, Guid secondId)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var link = await database.MemberTasks
                .Where(et => et.MemberId == firstId && et.TaskId == secondId)
                .FirstOrDefaultAsync();

            if (link == null) return false;

            database.MemberTasks.Remove(link);
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
            return await database.Tasks
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync() ?? new TaskDataModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new TaskDataModel();
        }
    }

    public async Task<IEnumerable<TaskDataModel>?> GetEntities()
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Tasks.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Enumerable.Empty<TaskDataModel>();
        }
    }

    public async Task<IEnumerable<TaskDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var tasksId = await database.PartTasks
                .Where(d => d.PartId == id)
                .Select(d => d.TaskId)
                .ToListAsync();

            return await database.Tasks
                .Where( t => tasksId.Contains(t.Id))
                .ToListAsync();
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
            var task = await database.Tasks.Where(t => t.Id == model.Id).FirstOrDefaultAsync();

            if (task == null) return false;

            if(!string.IsNullOrEmpty(model.Name))
                task.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Description))
                task.Description = model.Description;

            if (model.EmployeeId != Guid.Empty)
                task.EmployeeId = model.EmployeeId; 
            
            task.Level = model.Level;

            task.Status = model.Status;

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
            var existingTask = await database.Tasks.FindAsync(id);

            if (existingTask == null) return false;

            database.Tasks.Remove(existingTask);
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

    public async Task<IEnumerable<TaskDataModel>> GetFreeTasks(Guid projectId)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var entities = await GetEntitiesById(projectId);

            var tasksIds = await database.MemberTasks
                .Select(et => et.TaskId)
                .ToListAsync();

            if (entities == null) throw new NullReferenceException();

            return entities.Where(e => !tasksIds.Contains(e.Id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Enumerable.Empty<TaskDataModel>();
        }
    }

    public async Task<IEnumerable<TaskDataModel>> GetMemberTasks(Guid employeeId)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var taskIds = await database.MemberTasks
                .Where(et => et.MemberId == employeeId)
                .Select(et => et.TaskId)
                .ToListAsync();

            return await database.Tasks
                .Where(t => taskIds.Contains(t.Id))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Enumerable.Empty<TaskDataModel>();
        }
    }
}