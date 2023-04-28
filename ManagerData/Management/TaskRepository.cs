
using ManagerData.DataModels;

namespace ManagerData.Management;

public class TaskRepository : IManagementRepository<TaskDataModel>
{
    public Task<bool> CreateEntity(TaskDataModel model)
    {
        throw new NotImplementedException();
    }

    public Task<TaskDataModel> GetEntityById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateEntity(TaskDataModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }
}