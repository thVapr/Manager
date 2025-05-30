using ManagerData.DataModels;

namespace ManagerData.Management;

public interface ITaskMessageRepository
{
    Task<ICollection<TaskMessage>> GetTaskMessages(Guid taskId);
    Task<bool> CreateAsync(TaskMessage message);
    Task<bool> DeleteAsync(Guid messageId);
}