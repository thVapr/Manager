namespace ManagerLogic.Management;

public interface IManagementLogic<T>
{
    Task<T> GetEntityById(Guid id);
    Task<ICollection<T>> GetEntities();
    Task<ICollection<T>> GetEntitiesById(Guid id);

    Task<bool> CreateEntity(T model);
    Task<bool> UpdateEntity(T model);
    Task<bool> AddToEntity(Guid destinationId, Guid sourceId);
    Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId);
    Task<bool> LinkEntities(Guid masterId, Guid slaveId);
    Task<bool> UnlinkEntities(Guid masterId, Guid slaveId);
    
    Task<ICollection<T>> GetEntitiesByQuery(string query, Guid id);

    Task<bool> DeleteEntity(Guid id);
}