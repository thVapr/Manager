
using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IMemberRepository : IManagementRepository<MemberDataModel>
{
    Task<IEnumerable<MemberDataModel>> GetMembersWithoutPart();
    Task<IEnumerable<MemberDataModel>> GetMembersFromPart(Guid id);
    Task<IEnumerable<MemberDataModel>> GetAvailableMembersFromPart(Guid id);
}