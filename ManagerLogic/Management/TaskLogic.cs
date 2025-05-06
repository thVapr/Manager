using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class TaskLogic(ITaskRepository repository) : ITaskLogic
{
    public async Task<TaskModel> GetEntityById(Guid id)
    {
        var task = await repository.GetEntityById(id);
        return ConvertToLogicModel(task!);
    }

    public async Task<ICollection<TaskModel>> GetEntities()
    {
        var entities = await repository.GetEntities();

        if (entities == null) return [];

        return entities.Select(task => ConvertToLogicModel(task!)).ToList();
    }

    public async Task<ICollection<TaskModel>> GetEntitiesById(Guid id)
    {
        var tasks = await repository.GetEntitiesById(id);

        if (tasks == null) return [];

        return tasks.Select(task => ConvertToLogicModel(task!)).ToList();
    }

    public async Task<bool> CreateEntity(TaskModel model)
    {
        var entity = new TaskDataModel
        {
            Id = Guid.NewGuid(),

            Name = model.Name!,
            Description = model.Description ?? "",

            CreatorId = model.CreatorId,
            PartId = model.PartId,
            
            StartTime = model.StartTime,
            Deadline = model.Deadline,
            ClosedAt = model.ClosedAt,
            
            Status = model.Status,
            Level = model.Level,
            Priority = model.Priority,
        };

        return await repository.CreateEntity((Guid)model.PartId!, entity);
    }

    public async Task<bool> UpdateEntity(TaskModel model)
    {
        return await repository.UpdateEntity(new TaskDataModel
        {
            Id = Guid.Parse(model.Id ?? ""),
            Name = model.Name!,
            Description = model.Description!,

            StartTime = model.StartTime,
            Deadline = model.Deadline,
            ClosedAt = model.ClosedAt,
            
            Priority = model.Priority,
            Level = model.Level,
            Status = model.Status,
        });
    }

    public Task<bool> AddToEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UnlinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<TaskModel>> GetEntitiesByQuery(string query, Guid id)
    {
        var entities = await GetEntitiesById(id);

        var queries = query.ToLower().Split(' ');

        return entities.Where(e => 
            queries.Any(q => e.Name!.Contains(q, StringComparison.CurrentCultureIgnoreCase)) &&
                                   !string.IsNullOrEmpty(e.Name) ||
            queries.Any(q => e.Description!.Contains(q, StringComparison.CurrentCultureIgnoreCase)) &&
                                   !string.IsNullOrEmpty(e.Description)
            ).ToList();
    }

    public async Task<bool> DeleteEntity(Guid id)
    {
        return await repository.DeleteEntity(id);
    }

    public async Task<bool> AddMemberToTask(Guid memberId, Guid taskId, int groupId)
    {
        await repository.UpdateEntity(new TaskDataModel
        {
            Id = taskId,
            Status = (int)PublicConstants.Task.Doing
        });

        return await repository.AddToEntity(memberId, taskId);
    }

    public async Task<bool> RemoveMemberFromTask(Guid employeeId, Guid taskId)
    {
        await repository.UpdateEntity(new TaskDataModel
        {
            Id = taskId,
            Status = (int)PublicConstants.Task.Todo
        });

        return await repository.RemoveFromEntity(employeeId, taskId);
    }

    public async Task<ICollection<TaskModel>> GetFreeTasks(Guid projectId)
    {
        var tasks = await repository.GetFreeTasks(projectId);

        return tasks.Select(task => ConvertToLogicModel(task!)).ToList();
    }

    public async Task<ICollection<TaskModel>> GetMemberTasks(Guid employeeId)
    {
        var tasks = await repository.GetMemberTasks(employeeId);

        return tasks.Select(task => ConvertToLogicModel(task!)).ToList();
    }

    private TaskModel ConvertToLogicModel(TaskDataModel model)
    {
        return new TaskModel
        {
            Id = model.Id.ToString(),
            
            Name = model.Name,
            Description = model.Description,
            
            CreatorId = model.CreatorId,
            PartId = model.PartId,
            
            StartTime = model.StartTime,
            Deadline = model.Deadline,
            ClosedAt = model.ClosedAt,
            
            Level = model.Level,
            Status = model.Status,
            Priority = model.Priority,
        };
    }
}