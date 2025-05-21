using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IPartRepository : IManagementRepository<PartDataModel>
{ 
    Task<List<PartDataModel>> GetLinks(Guid partId);
    Task<IEnumerable<PartMemberDataModel>> GetPartMembers(Guid partId);
    Task<bool> SetPrivileges(Guid userId, Guid partId, int privilege);
    Task<ICollection<PartType>> GetPartTypes();
    
    Task<bool> AddPartTaskStatus(PartTaskStatus status);
    Task<bool> ChangePartTaskStatus(PartTaskStatus status);
    Task<bool> RemovePartTaskStatus(Guid partId, Guid partTaskStatusId);
    Task<ICollection<PartTaskStatus>> GetPartTaskStatuses(Guid partId);
}