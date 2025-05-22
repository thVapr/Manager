using ManagerData.DataModels;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskLogic : IManagementLogic<TaskModel>
{
    Task<ICollection<TaskHistoryModel>> GetTaskHistory(Guid taskId);
    Task<bool> UpdateTask(HistoryModel? historyModel, TaskModel taskModel);
    Task<bool> AddMemberToTask(Guid initiatorId, Guid memberId, Guid taskId, int groupId);
    Task<bool> RemoveMemberFromTask(Guid initiatorId, Guid memberId, Guid taskId);
    Task<bool> ChangeTaskStatus(HistoryModel historyModel, Guid taskId, bool forward);
    Task<ICollection<TaskModel>> GetFreeTasks(Guid partId);
    Task<ICollection<TaskModel>> GetAvailableTasks(Guid memberId, Guid partId);
    Task<ICollection<MemberModel>> GetAvailableMembersForTask(Guid partId, Guid taskId, bool includeExecutors = false);
    Task<ICollection<TaskModel>> GetMemberTasks(Guid memberId);
    Task<ICollection<MemberModel>> GetTaskMembers(Guid taskId);
}