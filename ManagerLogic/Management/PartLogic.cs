using ManagerLogic.Models;
using ManagerData.DataModels;
using ManagerData.Management;

using PartType = ManagerLogic.Models.PartType;

namespace ManagerLogic.Management;

public class PartLogic(IPartRepository repository) : IPartLogic
{
    public async Task<PartModel> GetEntityById(Guid id)
    {
        var entity = await repository.GetEntityById(id);

        if (entity.Id == Guid.Empty) return new PartModel();

        return ConvertDataModelToLogic(entity);
    }

    public async Task<ICollection<PartModel>> GetEntities()
    {
        var entity = await repository.GetEntities();
        
        return entity!.Where(e => e.Level == 0).Select(e =>
            ConvertDataModelToLogic(e!)).ToList() ?? [];
    }

    public async Task<ICollection<PartModel>> GetEntitiesById(Guid id)
    {
        var entities = await repository.GetEntitiesById(id);
        var result = new List<PartModel>();

        if (entities == null) return result;

        result.AddRange(entities.Where(e => e.Id != Guid.Empty).Select(e => 
            ConvertDataModelToLogic(e!))
        );

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

        if (model.MainPartId.HasValue)
        {
            return await repository.CreateEntity((Guid)model.MainPartId, entity);
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

    public Task<ICollection<PartModel>> GetEntitiesByQuery(string query, Guid id)
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
        return usersPrivileges.Any(up => up >= privilege);
    }

    public async Task<bool> UpdateHierarchy(ICollection<PartModel> models)
    {
        foreach (var model in models)
        {
            await UpdateOneNode(model);
        }
        return true;
    }

    public async Task<bool> CreatePart(Guid userId, PartModel model)
    {
        var id = Guid.NewGuid();
        var entity = new PartDataModel
        {
            Id = id,
            Name = model.Name!,
            Description = model.Description!,
            Level = model.Level!,
            TypeId = model.TypeId,
        };

        if (model.MainPartId.HasValue)
        {
            return await repository.CreateEntity((Guid)model.MainPartId, entity);
        }
        var isEntityCreated = await repository.CreateEntity(entity);
        if (!isEntityCreated) 
            return false;
        
        var part = await repository.GetEntityById(id);
        return await ChangePrivilege(userId, part.Id, 5);
    }

    private async Task UpdateOneNode(PartModel model)
    {
        if (model.Parts != null && model.Parts!.Count() != 0)
        {
            foreach (var part in model.Parts!)
            {
                await UpdateOneNode(part);
            }
        }
        await repository.UpdateEntity(new PartDataModel
        {
            Id = Guid.Parse(model.Id!),
            MainPartId = model.MainPartId,
            Level = model.Level,
        });
    }
    
    public async Task<ICollection<PartModel>> GetAllAccessibleParts(Guid userId)
    {
        var parts = await repository.GetEntities();
        var result = new List<PartModel>();

        foreach (var part in parts!)
        {
            if (part.Level == 0 && await IsUserHasPrivileges(userId, part.Id, 1))
            {
                result.Add(ConvertDataModelToLogic(part));
            }
        }

        return result;
    }

    public async Task<ICollection<PartType>> GetPartTypes()
    {
        var types = await repository.GetPartTypes();
        return types.Select(type => new PartType
        {
            Id = type.Id,
            Name = type.Name!
        }).ToList();
    }

    private PartModel ConvertDataModelToLogic(PartDataModel model)
    {
        return new PartModel
        {
            Id = model.Id.ToString(),
            Name = model.Name,
            Description = model.Description,
            Level = model.Level,
            TypeId = model.TypeId,
            MainPartId = model.MainPartId,
            Parts = model.Parts.Select(ConvertDataModelToLogic).ToList()
        };
    }
}