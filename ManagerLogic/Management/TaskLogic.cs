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

        var task = new TaskModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatorId = entity.CreatorId,
            ProjectId = entity.ProjectId,
            EmployeeId = entity.EmployeeId,
        };

        return task;
    }

    public Task<IEnumerable<TaskModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TaskModel>> GetEntitiesById(Guid id)
    {
        var entities = await _repository.GetEntitiesById(id);

        if (entities == null) return Enumerable.Empty<TaskModel>();

        return entities.Select(e => new TaskModel
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            CreatorId = e.CreatorId,
            ProjectId = e.ProjectId,
            EmployeeId = e.EmployeeId,
        }).ToList();
    }

    public async Task<bool> CreateEntity(TaskModel model)
    {
        var entity = new TaskDataModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            CreatorId = model.CreatorId,
            ProjectId = model.ProjectId,
        };

        return await _repository.CreateEntity(model.ProjectId, entity);
    }

    public Task<bool> UpdateEntity(TaskModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddEmployeeToTask(Guid employeeId, Guid taskId)
    {
        return _repository.LinkEntities(employeeId, taskId);
    }

    public async Task<IEnumerable<TaskModel>> GetFreeTasks(Guid projectId)
    {
        var tasks = await _repository.GetFreeTasks(projectId);

        return tasks.Select(t => new TaskModel
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            CreatorId = t.CreatorId,
            ProjectId = t.ProjectId,
            EmployeeId = t.EmployeeId,
        });
    }

    public async Task<IEnumerable<TaskModel>> GetEmployeeTasks(Guid employeeId)
    {
        var tasks = await _repository.GetEmployeeTasks(employeeId);

        return tasks.Select(t => new TaskModel
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            CreatorId = t.CreatorId,
            ProjectId = t.ProjectId,
            EmployeeId = t.EmployeeId,
        });
    }
}