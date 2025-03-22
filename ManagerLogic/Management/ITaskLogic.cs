using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskLogic : IManagementLogic<TaskModel>
{
    Task<bool> AddMemberToTask(Guid employeeId, Guid taskId);
    Task<bool> RemoveMemberFromTask(Guid employeeId, Guid taskId);
    //TODO: Добавить AddMemberToNotify

    Task<IEnumerable<TaskModel>> GetFreeTasks(Guid projectId);
    Task<IEnumerable<TaskModel>> GetMemberTasks(Guid employeeId);

}