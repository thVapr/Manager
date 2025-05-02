using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IPartRepository : IManagementRepository<PartDataModel>
{ 
    Task<List<PartDataModel>> GetLinks(Guid partId);
    Task<IEnumerable<PartMemberDataModel>> GetPartMembers(Guid partId);
    Task<bool> SetPrivileges(Guid userId, Guid partId, int privilege);
    Task<ICollection<PartType>> GetPartTypes();
}