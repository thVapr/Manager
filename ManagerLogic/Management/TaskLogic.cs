using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class TaskLogic : ITaskLogic
{
    private readonly ITaskRepository _repository;

    public TaskLogic(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        return new TaskModel
        {
            Id = entity.Id.ToString(),

            Name = entity.Name,
            Description = entity.Description,
            
            CreatorId = entity.CreatorId,
            ProjectId = entity.ProjectId,
            EmployeeId = entity.EmployeeId,

            Level = entity.Level,
            Status = entity.Status,
        };
    }

    public async Task<IEnumerable<TaskModel>> GetEntities()
    {
        var entities = await _repository.GetEntities();

        if (entities == null) return Enumerable.Empty<TaskModel>();

        return entities.Select(e => new TaskModel
        {
            Id = e.Id.ToString(),

            Name = e.Name,
            Description = e.Description,

            CreatorId = e.CreatorId,
            ProjectId = e.ProjectId,
            EmployeeId = e.EmployeeId,

            Level = e.Level,
            Status = e.Status,
        }).ToList();
    }

    public async Task<IEnumerable<TaskModel>> GetEntitiesById(Guid id)
    {
        var entities = await _repository.GetEntitiesById(id);

        if (entities == null) return Enumerable.Empty<TaskModel>();

        return entities.Select(e => new TaskModel
        {
            Id = e.Id.ToString(),

            Name = e.Name,
            Description = e.Description,

            CreatorId = e.CreatorId,
            ProjectId = e.ProjectId,
            EmployeeId = e.EmployeeId,

            Level = e.Level,
            Status = e.Status,
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
            ProjectId = model.ProjectId,

            Level = model.Level,
        };

        return await _repository.CreateEntity(model.ProjectId, entity);
    }

    public async Task<bool> UpdateEntity(TaskModel model)
    {
        return await _repository.UpdateEntity(new TaskDataModel
        {
            Id = Guid.Parse(model.Id ?? ""),
            Name = model.Name!,
            Description = model.Description!,

            EmployeeId = model.EmployeeId,

            Level = model.Level,
            Status = model.Status,
        });
    }

    public async Task<IEnumerable<TaskModel>> GetEntitiesByQuery(string query, Guid id)
    {
        var entities = await GetEntitiesById(id);

        var queries = query.ToLower().Split(' ');

        return entities.Where(e => 
            queries.Any(q => e.Name!.ToLower().Contains(q)) &&
                                   !string.IsNullOrEmpty(e.Name) ||
            queries.Any(q => e.Description!.ToLower().Contains(q)) &&
                                   !string.IsNullOrEmpty(e.Description)
            );
    }

    public async Task<bool> DeleteEntity(Guid id)
    {
        return await _repository.DeleteEntity(id);
    }

    public async Task<bool> AddMemberToTask(Guid employeeId, Guid taskId)
    {
        await _repository.UpdateEntity(new TaskDataModel
        {
            Id = taskId,
            EmployeeId = employeeId,
            Status = (int)PublicConstants.Task.Doing
        });

        return await _repository.LinkEntities(employeeId, taskId);
    }

    public async Task<bool> RemoveMemberFromTask(Guid employeeId, Guid taskId)
    {
        await _repository.UpdateEntity(new TaskDataModel
        {
            Id = taskId,
            Status = (int)PublicConstants.Task.Todo
        });

        return await _repository.UnlinkEntities(employeeId, taskId);
    }

    public async Task<IEnumerable<TaskModel>> GetFreeTasks(Guid projectId)
    {
        var tasks = await _repository.GetFreeTasks(projectId);

        return tasks.Select(t => new TaskModel
        {
            Id = t.Id.ToString(),

            Name = t.Name,
            Description = t.Description,
            
            CreatorId = t.CreatorId,
            ProjectId = t.ProjectId,
            EmployeeId = t.EmployeeId,

            Level = t.Level,
            Status = t.Status
        });
    }

    public async Task<IEnumerable<TaskModel>> GetMemberTasks(Guid employeeId)
    {
        var tasks = await _repository.GetMemberTasks(employeeId);

        return tasks.Select(t => new TaskModel
        {
            Id = t.Id.ToString(),
            
            Name = t.Name,
            Description = t.Description,
            
            CreatorId = t.CreatorId,
            ProjectId = t.ProjectId,
            EmployeeId = t.EmployeeId,

            Level = t.Level,
            Status = t.Status,
        });
    }
}