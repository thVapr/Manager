using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IBackgroundTaskRepository
{
    Task<bool> Create(BackgroundTask task);
    Task<bool> Delete(Guid id);
    Task<ICollection<BackgroundTask>> GetAllNearest();
}