
using ManagerData.DataModels;

namespace ManagerData.Management;

public interface ITaskRepository
{
    public Task<bool> CreateTask(TaskDataModel model);
    public Task<TaskDataModel> GetTask(Guid id);
    public Task<bool> UpdateTask(TaskDataModel model);
    public Task<bool> DeleteTask(Guid id);
}