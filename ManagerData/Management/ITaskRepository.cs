
using ManagerData.DataModels;

namespace ManagerData.Management;

public interface ITaskRepository : IManagementRepository<TaskDataModel>
{
    Task<IEnumerable<TaskDataModel>> GetFreeTasks(Guid projectId);
    Task<IEnumerable<TaskDataModel>> GetEmployeeTasks(Guid employeeId);
}