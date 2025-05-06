
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class TaskRepository : ITaskRepository
{
    public async Task<bool> CreateEntity(TaskDataModel model)
    {
        await using var database = new MainDbContext();

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
        await using var database = new MainDbContext();

        try
        {
            var part = database.Parts.FirstOrDefault(p => p.Id == id);
            if (part is null) 
                return false;
            var task = await database.Tasks
                .FirstOrDefaultAsync(t => t.Id == model.Id);
            if (task is null)
                return false;
            
            task!.PartId = id;
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
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
            await database.TaskMembers.AddAsync(new TaskMember()
            {
                MemberId = destinationId,
                TaskId = sourceId,
                ParticipationPurpose = 1
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

    public async Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId)
    {
        await using var database = new MainDbContext();

        try
        {
            var link = await database.TaskMembers
                .Where(et => et.MemberId == destinationId && et.TaskId == sourceId)
                .FirstOrDefaultAsync();

            if (link == null) return false;

            database.TaskMembers.Remove(link);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public Task<bool> LinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> UnlinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public async Task<TaskDataModel> GetEntityById(Guid id)
    {
        await using var database = new MainDbContext();

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
        await using var database = new MainDbContext();

        try
        {
            return await database.Tasks.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }

    public async Task<IEnumerable<TaskDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var tasksId = await database.Tasks
                .Where(d => d.PartId == id)
                .Select(d => d.Id)
                .ToListAsync();

            return await database.Tasks
                .Where( t => tasksId.Contains(t.Id))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }

    public async Task<bool> UpdateEntity(TaskDataModel model)
    {
        await using var database = new MainDbContext();

        try
        {
            var task = await database.Tasks
                .Where(t => t.Id == model.Id)
                .FirstOrDefaultAsync();

            if (task == null) 
                return false;

            if(!string.IsNullOrEmpty(model.Name))
                task.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Description))
                task.Description = model.Description;
            if (model.StartTime.HasValue)
                model.Deadline = model.Deadline;
            if (model.Deadline.HasValue)
                model.Deadline = model.Deadline;
            if (model.ClosedAt.HasValue)
                model.ClosedAt = model.ClosedAt;
            if (model.Level >= 0)
                task.Level = model.Level;
            if (model.Status >= 0)
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
        await using var database = new MainDbContext();

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
    
    public async Task<IEnumerable<TaskDataModel>> GetFreeTasks(Guid projectId)
    {
        await using var database = new MainDbContext();

        try
        {
            var entities = await GetEntitiesById(projectId);

            var tasksIds = await database.TaskMembers
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
        await using var database = new MainDbContext();

        try
        {
            var taskIds = await database.TaskMembers
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
            return [];
        }
    }
    
    public void Dispose()
    {
    }
}