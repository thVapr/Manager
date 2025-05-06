using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskLogic : IManagementLogic<TaskModel>
{
    Task<bool> AddMemberToTask(Guid memberId, Guid taskId, int groupId);
    Task<bool> RemoveMemberFromTask(Guid memberId, Guid taskId);
    Task<bool> ChangeTaskStatus(Guid taskId);
    Task<ICollection<TaskModel>> GetFreeTasks(Guid projectId);
    Task<ICollection<TaskModel>> GetMemberTasks(Guid memberId);
}