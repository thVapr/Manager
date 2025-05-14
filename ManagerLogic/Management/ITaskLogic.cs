using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskLogic : IManagementLogic<TaskModel>
{
    Task<bool> UpdateTask(HistoryModel? historyModel, TaskModel taskModel);
    Task<bool> AddMemberToTask(Guid memberId, Guid taskId, int groupId);
    Task<bool> RemoveMemberFromTask(Guid memberId, Guid taskId);
    Task<bool> ChangeTaskStatus(HistoryModel historyModel, Guid taskId);
    Task<ICollection<TaskModel>> GetFreeTasks(Guid partId);
    Task<ICollection<TaskModel>> GetAvailableTasks(Guid memberId, Guid partId);
    Task<ICollection<TaskModel>> GetMemberTasks(Guid memberId);
    Task<ICollection<MemberModel>> GetTaskMembers(Guid taskId);
}