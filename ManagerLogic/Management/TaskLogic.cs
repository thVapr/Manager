using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class TaskLogic : ITaskLogic
{
    private readonly IManagementRepository<TaskDataModel> _repository;

    public TaskLogic(IManagementRepository<TaskDataModel> repository)
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
        };

        return task;
    }

    public Task<IEnumerable<TaskModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskModel>> GetEntitiesById(Guid id)
    {
        throw new NotImplementedException();
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
}