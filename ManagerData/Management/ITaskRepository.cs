
using ManagerData.DataModels;

namespace ManagerData.Management;

public interface ITaskRepository
{
    public Task<TaskDataModel> GetTask(string id);
    public Task<bool> CreateTask(TaskDataModel model);
    public Task<bool> UpdateTask(TaskDataModel model);
    public Task<bool> DeleteTask(string id);
}