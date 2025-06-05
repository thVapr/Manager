using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IRoleRepository
{
    Task<bool> Create(PartRole role);
    Task<bool> Change(PartRole role);
    Task<bool> Delete(Guid partId, Guid roleId);
    
    Task<bool> SetRole(Guid partId, Guid roleId, Guid memberId);
    Task<bool> RemoveRole(Guid partId, Guid roleId, Guid memberId);
    
    Task<ICollection<PartRole>> GetByMemberId(Guid partId, Guid memberId);
    Task<ICollection<PartRole>> GetByPartId(Guid partId);
}