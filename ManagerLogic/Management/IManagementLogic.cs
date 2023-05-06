namespace ManagerLogic.Management;

public interface IManagementLogic<T>
{
    Task<T> GetEntityById(Guid id);

    Task<bool> CreateEntity(T model);
    Task<bool> UpdateEntity(T model);

    Task<bool> DeleteEntity(Guid id);
}