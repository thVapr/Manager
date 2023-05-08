using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskLogic : IManagementLogic<TaskModel>
{
    Task<bool> AddEmployeeToTask(Guid employeeId, Guid taskId);
}