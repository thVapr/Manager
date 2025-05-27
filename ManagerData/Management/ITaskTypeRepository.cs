using ManagerData.DataModels;

namespace ManagerData.Management;

public interface ITaskTypeRepository
{
    Task<bool> Create(PartTaskType type);
    Task<bool> Change(PartTaskType type);
    Task<bool> Delete(Guid partId, Guid typeId);

    Task<ICollection<PartTaskType>> GetByPartId(Guid partId);
}