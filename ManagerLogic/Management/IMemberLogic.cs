
using ManagerData.DataModels;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IMemberLogic : IManagementLogic<MemberModel>
{
    Task<ICollection<MemberModel>> GetAvailableMembers(Guid? partId);
    Task<ICollection<MemberModel>> GetMembersWithoutPart();
    Task<ICollection<MemberModel>> GetFreeMembersInPart(Guid id);

    Task<ICollection<MemberModel>> GetMembersFromPart(Guid id);
}