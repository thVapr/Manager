namespace ManagerData.Management;

public interface IManagementRepository<T> : IDisposable
{
    Task<bool> CreateEntity(T model);
    Task<bool> CreateEntity(Guid id, T model);

    Task<bool> AddToEntity(Guid destinationId, Guid sourceId);
    Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId);

    Task<bool> LinkEntities(Guid masterId, Guid slaveId);
    Task<bool> UnlinkEntities(Guid masterId, Guid slaveId);
    
    Task<T> GetEntityById(Guid id);
    Task<IEnumerable<T>?> GetEntities();
    Task<IEnumerable<T>?> GetEntitiesById(Guid id);

    Task<bool> UpdateEntity(T model);
    Task<bool> DeleteEntity(Guid id);
}