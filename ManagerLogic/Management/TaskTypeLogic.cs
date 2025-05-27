using ManagerLogic.Models;
using ManagerData.DataModels;
using ManagerData.Management;

namespace ManagerLogic.Management;

public class TaskTypeLogic(ITaskTypeRepository repository) : ITaskTypeLogic
{
    public async Task<bool> AddTaskTypeToPart(Guid partId, string name)
    {
        return await repository.Create(new PartTaskType
        {
            PartId = partId,
            Name = name,
        });
    }

    public async Task<bool> RemoveTaskTypeFromPart(Guid partId, Guid typeId)
    {
        return await repository.Delete(partId, typeId);
    }

    public async Task<ICollection<PartTaskTypeModel>> GetPartTaskTypes(Guid partId)
    {
        return (await repository.GetByPartId(partId)).Select(partTaskType => new PartTaskTypeModel
        {
            Id = partTaskType.Id.ToString(),
            Name = partTaskType.Name
        }).ToList();
    }
}