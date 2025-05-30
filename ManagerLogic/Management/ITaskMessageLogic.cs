using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskMessageLogic
{
    Task<ICollection<TaskMessageModel>> GetTaskMessages(Guid taskId);
    Task<bool> CreateAsync(TaskMessageModel message);
    Task<bool> DeleteAsync(Guid messageId);
}