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
            Name = entity.Name,
            Description = entity.Description,
        };

        return task;
    }

    public async Task<bool> CreateEntity(TaskModel model)
    {
        var entity = new TaskDataModel
        {
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