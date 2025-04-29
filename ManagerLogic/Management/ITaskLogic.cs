using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskLogic : IManagementLogic<TaskModel>
{
    Task<bool> AddMemberToTask(Guid memberId, Guid taskId);
    Task<bool> RemoveMemberFromTask(Guid memberId, Guid taskId);
    //TODO: Добавить AddMemberToNotify

    Task<ICollection<TaskModel>> GetFreeTasks(Guid projectId);
    Task<ICollection<TaskModel>> GetMemberTasks(Guid memberId);

}