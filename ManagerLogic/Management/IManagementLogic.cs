namespace ManagerLogic.Management;

public interface IManagementLogic<T>
{
    Task<bool> Create(T model);
    Task<T> GetById(Guid id);
    Task<ICollection<T>> GetAll();
    Task<ICollection<T>> GetManyById(Guid id);
    Task<ICollection<T>> GetByQuery(string query, Guid id);
    Task<bool> Update(T model);
    Task<bool> Delete(Guid id);

    Task<bool> AddTo(Guid destinationId, Guid sourceId);
    Task<bool> RemoveFrom(Guid destinationId, Guid sourceId);
    Task<bool> AddLink(Guid masterId, Guid slaveId);
    Task<bool> RemoveLink(Guid masterId, Guid slaveId);
}