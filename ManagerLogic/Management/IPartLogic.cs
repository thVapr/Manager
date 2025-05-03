using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IPartLogic : IManagementLogic<PartModel>
{
    Task<int> GetPrivileges(Guid userId, Guid partId);
    Task<bool> ChangePrivilege(Guid userId, Guid partId, int privilege);
    Task<bool> IsUserHasPrivileges(Guid userId, Guid partId, int privilege);
    
    Task<bool> UpdateHierarchy(ICollection<PartModel> models);
    
    Task<bool> CreatePart(Guid userId, PartModel model);
    
    Task<ICollection<PartModel>> GetAllAccessibleParts(Guid userId);
    
    Task<ICollection<PartType>> GetPartTypes();
}