using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class TaskLogic(ITaskRepository repository) : ITaskLogic
{
    public async Task<TaskModel> GetEntityById(Guid id)
    {
        var entity = await repository.GetEntityById(id);
        // TODO: В данном классе куча однотипных приведений
        return new TaskModel
        {
            Id = entity.Id.ToString(),

            Name = entity.Name,
            Description = entity.Description,
            
            CreatorId = entity.CreatorId,
            PartId = entity.PartId,
            MemberId = entity.MemberId,

            Level = entity.Level,
            Status = entity.Status,
            Priority = entity.Priority,
        };
    }

    public async Task<IEnumerable<TaskModel>> GetEntities()
    {
        var entities = await repository.GetEntities();

        if (entities == null) return [];

        return entities.Select(e => new TaskModel
        {
            Id = e.Id.ToString(),

            Name = e.Name,
            Description = e.Description,

            CreatorId = e.CreatorId,
            PartId = e.PartId,
            MemberId = e.PartId,

            Level = e.Level,
            Status = e.Status,
            Priority = e.Priority,
        }).ToList();
    }

    public async Task<IEnumerable<TaskModel>> GetEntitiesById(Guid id)
    {
        var entities = await repository.GetEntitiesById(id);

        if (entities == null) return [];

        return entities.Select(e => new TaskModel
        {
            Id = e.Id.ToString(),

            Name = e.Name,
            Description = e.Description,

            CreatorId = e.CreatorId,
            PartId = e.PartId,
            MemberId = e.PartId,

            Level = e.Level,
            Status = e.Status,
            Priority = e.Priority,
        }).ToList();
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

            MemberId = model.MemberId,

            Level = model.Level,
            Status = model.Status,
        });
    }

    public async Task<IEnumerable<TaskModel>> GetEntitiesByQuery(string query, Guid id)
    {
        var entities = await GetEntitiesById(id);

        var queries = query.ToLower().Split(' ');

        return entities.Where(e => 
            queries.Any(q => e.Name!.Contains(q, StringComparison.CurrentCultureIgnoreCase)) &&
                                   !string.IsNullOrEmpty(e.Name) ||
            queries.Any(q => e.Description!.Contains(q, StringComparison.CurrentCultureIgnoreCase)) &&
                                   !string.IsNullOrEmpty(e.Description)
            );
    }

    public async Task<bool> DeleteEntity(Guid id)
    {
        return await repository.DeleteEntity(id);
    }

    public async Task<bool> AddMemberToTask(Guid memberId, Guid taskId)
    {
        await repository.UpdateEntity(new TaskDataModel
        {
            Id = taskId,
            MemberId = memberId,
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

    public async Task<IEnumerable<TaskModel>> GetFreeTasks(Guid projectId)
    {
        var tasks = await repository.GetFreeTasks(projectId);

        return tasks.Select(t => new TaskModel
        {
            Id = t.Id.ToString(),

            Name = t.Name,
            Description = t.Description,
            
            CreatorId = t.CreatorId,
            PartId = t.PartId,
            MemberId = t.MemberId,

            Level = t.Level,
            Status = t.Status,
            Priority = t.Priority,
        });
    }

    public async Task<IEnumerable<TaskModel>> GetMemberTasks(Guid employeeId)
    {
        var tasks = await repository.GetMemberTasks(employeeId);

        return tasks.Select(t => new TaskModel
        {
            Id = t.Id.ToString(),
            
            Name = t.Name,
            Description = t.Description,
            
            CreatorId = t.CreatorId,
            PartId = t.PartId,
            MemberId = t.MemberId,

            Level = t.Level,
            Status = t.Status,
            Priority = t.Priority,
        });
    }
}