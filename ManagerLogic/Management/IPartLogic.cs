
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IPartLogic : IManagementLogic<PartModel>
{
    Task<bool> ChangePrivilege(Guid userId, Guid partId, int privilege);
    Task<bool> IsUserHasPrivileges(Guid userId, Guid partId, int privilege);
}