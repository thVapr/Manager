using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IPartRepository : IManagementRepository<PartDataModel>
{ 
    Task<IEnumerable<PartLink>> GetLinks(Guid partId);
    Task<IEnumerable<PartMembersDataModel>> GetPartMembers(Guid partId);
    Task<bool> SetPrivileges(Guid userId, Guid partId, int privilege);
}