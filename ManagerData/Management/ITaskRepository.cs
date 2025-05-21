
using ManagerData.DataModels;

namespace ManagerData.Management;

public interface ITaskRepository : IManagementRepository<TaskDataModel>
{
    Task<IEnumerable<TaskDataModel>> GetFreeTasks(Guid projectId);
    Task<IEnumerable<TaskDataModel>> GetMemberTasks(Guid employeeId);
    Task<IEnumerable<Guid>> GetTaskMembersIds(Guid taskId);
    Task<IEnumerable<TaskMember>> GetTaskMembers(Guid taskId);
    
}