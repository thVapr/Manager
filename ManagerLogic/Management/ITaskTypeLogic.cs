using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface ITaskTypeLogic
{
    Task<bool> AddTaskTypeToPart(Guid partId, string name);
    Task<bool> RemoveTaskTypeFromPart(Guid partId, Guid typeId);
    Task<ICollection<PartTaskTypeModel>> GetPartTaskTypes(Guid partId);
}