using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskLogic : IManagementLogic<TaskModel>
{
    Task<bool> AddMemberToTask(Guid memberId, Guid taskId);
    Task<bool> RemoveMemberFromTask(Guid memberId, Guid taskId);
    //TODO: Добавить AddMemberToNotify

    Task<IEnumerable<TaskModel>> GetFreeTasks(Guid projectId);
    Task<IEnumerable<TaskModel>> GetMemberTasks(Guid memberId);

}