
using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IMemberRepository : IManagementRepository<MemberDataModel>
{
    Task<IEnumerable<MemberDataModel>> GetMembersWithoutPart(int level);
    Task<IEnumerable<MemberDataModel>> GetMembersFromPart(Guid id);
}