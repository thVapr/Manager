using ManagerLogic.Models;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerData.Authentication;

namespace ManagerLogic.Management;

public class PartLogic(IPartRepository repository) : IPartLogic
{
    public async Task<PartModel> GetEntityById(Guid id)
    {
        var entity = await repository.GetEntityById(id);

        if (entity.Id == Guid.Empty) return new PartModel();

        return new PartModel
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
        };
    }

    public Task<IEnumerable<PartModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PartModel>> GetEntitiesById(Guid id)
    {
        var entities = await repository.GetEntitiesById(id);
        var result = new List<PartModel>();

        if (entities == null) return result;

        result.AddRange(entities.Where(e => e.Id != Guid.Empty).Select(e => new PartModel
        {
            Id = e.Id.ToString(),
            Name = e.Name,
            Description = e.Description,
        }));

        return result;
    }

    public async Task<bool> CreateEntity(PartModel model)
    {
        var entity = new PartDataModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name!,
            Description = model.Description!,
            Level = model.Level!,
            TypeId = model.TypeId,
        };

        if (model.MasterId.HasValue)
        {
            return await repository.CreateEntity((Guid)model.MasterId, entity);
        }
        return await repository.CreateEntity(entity);
    }

    public async Task<bool> UpdateEntity(PartModel model)
    {
        return await repository.UpdateEntity(new PartDataModel
        {
            Id = Guid.Parse(model.Id!),
            Name = model.Name!,
            Description = model.Description!,
        });
    }

    public async Task<bool> AddToEntity(Guid destinationId, Guid sourceId)
    {
        return await repository.AddToEntity(destinationId, sourceId);
    }

    public Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LinkEntities(Guid masterId, Guid slaveId)
    {
        return await repository.LinkEntities(masterId, slaveId);
    }

    public async Task<bool> UnlinkEntities(Guid masterId, Guid slaveId)
    {
        return await repository.UnlinkEntities(masterId, slaveId);
    }

    public Task<IEnumerable<PartModel>> GetEntitiesByQuery(string query, Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteEntity(Guid id)
    {
        return await repository.DeleteEntity(id);
    }

    public async Task<bool> ChangePrivilege(Guid userId, Guid partId, int privilege)
    {
        return await repository.SetPrivileges(userId, partId, privilege); 
    }

    public async Task<bool> IsUserHasPrivileges(Guid userId, Guid partId, int privilege)
    {
        var usersPrivileges = (await repository.GetPartMembers(partId))
            .Where(up => up.MemberId == userId)
            .Select(up => up.Privileges)
            .ToList();
        return usersPrivileges.Contains(privilege);
    }
}