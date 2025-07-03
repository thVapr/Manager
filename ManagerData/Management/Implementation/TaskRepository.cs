using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagerData.Management.Implementation;

public class TaskRepository(MainDbContext database,  ILogger<TaskRepository> logger) : ITaskRepository
{
    public async Task<bool> Create(TaskDataModel model)
    {
        try
        {
            await database.Tasks.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> Create(Guid id, TaskDataModel model)
    {
        if (!await Create(model)) return false;
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
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> AddTo(Guid destinationId, Guid sourceId)
    {
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
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> RemoveFrom(Guid destinationId, Guid sourceId)
    {
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
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public Task<bool> AddLink(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> RemoveLink(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public async Task<TaskDataModel> GetById(Guid id)
    {
        try
        {
            return await database.Tasks
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync() ?? new TaskDataModel();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return new TaskDataModel();
        }
    }

    public async Task<IEnumerable<TaskDataModel>?> GetAll()
    {
        try
        {
            return await database.Tasks.ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<IEnumerable<TaskDataModel>?> GetManyById(Guid id)
    {
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
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<bool> Update(TaskDataModel model)
    {
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
                task.StartTime = model.StartTime;
            if (model.Deadline.HasValue)
                task.Deadline = model.Deadline;
            if (model.ClosedAt.HasValue)
                task.ClosedAt = model.ClosedAt;
            if (model.Level >= 0)
                task.Level = model.Level;
            if (model.Status >= 0)
                task.Status = model.Status;
            if (!string.IsNullOrEmpty(model.Path))
                task.Path = model.Path;
            if (model.TaskTypeId != Guid.Empty)
                task.TaskTypeId = model.TaskTypeId;
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> Delete(Guid id)
    {
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
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }
    
    public async Task<IEnumerable<TaskDataModel>> GetFreeTasks(Guid projectId)
    {
        try
        {
            var entities = await GetManyById(projectId);

            var tasksIds = await database.TaskMembers
                .Select(et => et.TaskId)
                .ToListAsync();

            if (entities == null) throw new NullReferenceException();

            return entities.Where(e => !tasksIds.Contains(e.Id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<IEnumerable<TaskDataModel>> GetMemberTasks(Guid employeeId)
    {
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
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<IEnumerable<Guid>> GetTaskMembersIds(Guid taskId)
    {
        try
        {
            var memberIds = await database.TaskMembers
                .Where(et => et.TaskId == taskId)
                .Select(et => et.MemberId)
                .ToListAsync();

            return memberIds;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<IEnumerable<TaskMember>> GetTaskMembers(Guid taskId)
    {
        try
        {
            var members = await database.TaskMembers
                .Where(et => et.TaskId == taskId)
                .ToListAsync();

            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }
}