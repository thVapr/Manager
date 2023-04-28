using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IManagementRepository<T>
{
    public Task<bool> CreateEntity(T model);
    public Task<T> GetEntityById(Guid id);
    public Task<bool> UpdateEntity(T model);
    public Task<bool> DeleteEntity(Guid id);
}