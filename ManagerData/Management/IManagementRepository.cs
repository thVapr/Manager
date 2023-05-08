
namespace ManagerData.Management;

public interface IManagementRepository<T> : IDisposable
{
    Task<bool> CreateEntity(T model);
    Task<bool> CreateEntity(Guid id, T model);
    Task<T> GetEntityById(Guid id);
    Task<bool> UpdateEntity(T model);
    Task<bool> DeleteEntity(Guid id);
}