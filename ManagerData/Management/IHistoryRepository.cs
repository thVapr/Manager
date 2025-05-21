using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IHistoryRepository
{
    Task<bool> Create(TaskHistory model);
    Task<ICollection<TaskHistory>> GetByTaskId(Guid taskId);
    Task<ICollection<TaskHistory>> GetByMemberId(Guid memberId);
}