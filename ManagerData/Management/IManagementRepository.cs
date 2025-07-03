namespace ManagerData.Management;

public interface IManagementRepository<T>
{
    Task<bool> Create(T model);
    Task<bool> Create(Guid id, T model);

    Task<T> GetById(Guid id);
    Task<IEnumerable<T>?> GetAll();
    Task<IEnumerable<T>?> GetManyById(Guid id);
    Task<bool> Update(T model);
    Task<bool> Delete(Guid id);

    Task<bool> AddTo(Guid destinationId, Guid sourceId);
    Task<bool> RemoveFrom(Guid destinationId, Guid sourceId);
    Task<bool> AddLink(Guid masterId, Guid slaveId);
    Task<bool> RemoveLink(Guid masterId, Guid slaveId);

}