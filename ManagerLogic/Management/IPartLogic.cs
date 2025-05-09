using ManagerData.DataModels;
using ManagerLogic.Models;
using PartType = ManagerLogic.Models.PartType;

namespace ManagerLogic.Management;

public interface IPartLogic : IManagementLogic<PartModel>
{
    Task<bool> AddRoleToPart(Guid partId, string name, string description);
    Task<bool> RemoveRoleFromPart(Guid partId, Guid roleId);
    Task<ICollection<PartRole>> GetPartRoles(Guid partId);
    Task<ICollection<PartRole>> GetPartMemberRoles(Guid partId, Guid memberId);
    Task<bool> AddMemberToRole(Guid partId, Guid memberId, Guid roleId);
    Task<bool> RemoveMemberFromRole(Guid partId, Guid memberId, Guid roleId);
    
    
    Task<int> GetPrivileges(Guid userId, Guid partId);
    Task<bool> ChangePrivilege(Guid userId, Guid partId, int privilege);
    Task<bool> IsUserHasPrivileges(Guid userId, Guid partId, int privilege);
    
    Task<bool> UpdateHierarchy(ICollection<PartModel> models);
    
    Task<bool> CreatePart(Guid userId, PartModel model);
    
    Task<ICollection<PartModel>> GetAllAccessibleParts(Guid userId);
    
    Task<ICollection<PartType>> GetPartTypes();
    Task<ICollection<PartTaskStatus>> GetPartTaskStatuses(Guid partId);
}