
using ManagerData.DataModels;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IMemberLogic : IManagementLogic<MemberModel>
{
    Task<IEnumerable<MemberModel>> GetMembersWithoutPart();
    Task<IEnumerable<MemberModel>> GetFreeMembersInPart(Guid id);

    Task<IEnumerable<MemberModel>> GetMembersFromPart(Guid id);
}