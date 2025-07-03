using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;
using PartType = ManagerLogic.Models.PartType;

namespace ManagerLogic.Management.Implementation;

public class PartLogic(IPartRepository repository, IRoleRepository roleRepository, IPathHelper pathHelper) : IPartLogic
{
    public async Task<PartModel> GetById(Guid id)
    {
        var entity = await repository.GetById(id);

        if (entity.Id == Guid.Empty) return new PartModel();

        return ConvertDataModelToLogic(entity);
    }

    public async Task<ICollection<PartModel>> GetAll()
    {
        var entity = await repository.GetAll();
        
        return entity!.Where(e => e.Level == 0)
            .Select(ConvertDataModelToLogic).ToList();
    }

    public async Task<ICollection<PartModel>> GetManyById(Guid id)
    {
        var entities = await repository.GetManyById(id);
        var result = new List<PartModel>();

        if (entities == null) return result;

        result.AddRange(entities
            .Where(e => e.Id != Guid.Empty)
            .Select(ConvertDataModelToLogic)
        );

        return result;
    }

    public async Task<bool> Create(PartModel model)
    {
        var entity = new PartDataModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name!,
            Description = model.Description!,
            Level = model.Level,
            PartTypeId = model.TypeId,
        };

        if (model.MainPartId.HasValue)
        {
            return await repository.Create((Guid)model.MainPartId, entity);
        }
        return await repository.Create(entity);
    }

    public async Task<bool> Update(PartModel model)
    {
        return await repository.Update(new PartDataModel
        {
            Id = Guid.Parse(model.Id!),
            Name = model.Name!,
            Description = model.Description!,
        });
    }

    public async Task<bool> AddTo(Guid destinationId, Guid sourceId)
    {
        return await repository.AddTo(destinationId, sourceId);
    }

    public async Task<bool> RemoveFrom(Guid destinationId, Guid sourceId)
    {
        return await repository.RemoveFrom(destinationId, sourceId);
    }

    public async Task<bool> AddLink(Guid masterId, Guid slaveId)
    {
        return await repository.AddLink(masterId, slaveId);
    }

    public async Task<bool> RemoveLink(Guid masterId, Guid slaveId)
    {
        return await repository.RemoveLink(masterId, slaveId);
    }

    public Task<ICollection<PartModel>> GetByQuery(string query, Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Delete(Guid id)
    {
        return await repository.Delete(id);
    }

    public async Task<bool> AddRoleToPart(Guid partId, string name, string description)
    {
        return await roleRepository.Create(new PartRole
        {
            PartId = partId,
            Name = name,
            Description = description,
        });
    }

    public async Task<bool> RemoveRoleFromPart(Guid partId, Guid roleId)
    {
        return await roleRepository.Delete(partId, roleId);
    }

    public async Task<ICollection<PartRole>> GetPartRoles(Guid partId)
    {
        return await roleRepository.GetByPartId(partId);
    }

    public async Task<ICollection<PartRole>> GetPartMemberRoles(Guid partId, Guid memberId)
    {
        return await roleRepository.GetByMemberId(partId, memberId);
    }

    public async Task<bool> AddMemberToRole(Guid partId, Guid memberId, Guid roleId)
    {
        return await roleRepository.SetRole(partId, roleId, memberId);
    }

    public async Task<bool> RemoveMemberFromRole(Guid partId, Guid memberId, Guid roleId)
    {
        return await roleRepository.RemoveRole(partId, roleId, memberId);
    }

    public async Task<int> GetPrivileges(Guid userId, Guid partId)
    {
        return (await repository.GetPartMembers(partId))
            .Where(pm => pm.MemberId == userId)
            .Select(pm => pm.Privileges).FirstOrDefault();
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
            Description = model.Description ?? string.Empty,
            Level = model.Level,
            PartTypeId = model.TypeId,
        };

        var isEntityCreated = model.MainPartId.HasValue 
            ? await repository.Create((Guid)model.MainPartId, entity)
            : await repository.Create(entity);
        if (!isEntityCreated) 
            return false;
        
        var part = await repository.GetById(id);
        return await ChangePrivilege(userId, part.Id, (int)AccessLevel.Leader);
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
        await repository.Update(new PartDataModel
        {
            Id = Guid.Parse(model.Id!),
            MainPartId = model.MainPartId,
            Level = model.Level,
        });
    }
    
    public async Task<ICollection<PartModel>> GetAllAccessibleParts(Guid userId)
    {
        var parts = await repository.GetAll();
        var result = new List<PartModel>();
        var minimumLevel = int.MaxValue;
        foreach (var part in parts!)
        {
            if (await IsUserHasPrivileges(userId, part.Id, 1))
            {
                minimumLevel = Math.Min(minimumLevel, part.Level);
                result.Add(ConvertDataModelToLogic(part));
            }
        }
        return result.Where(r => r.Level == minimumLevel).ToList();
    }

    public async Task<ICollection<PartType>> GetPartTypes()
    {
        var types = await repository.GetPartTypes();
        return types.Select(type => new PartType
        {
            Id = type.Id,
            Name = type.Name
        }).ToList();
    }

    public async Task<bool> AddPartTaskStatus(PartTaskStatusModel status)
    {
        Guid.TryParse(status.PartRoleId, out var partRoleId);
        
        return await repository.AddPartTaskStatus(new PartTaskStatus
        {
            GlobalStatus = status.GlobalStatus ?? -1,
            Name = status.Name!,
            PartId = Guid.Parse(status.PartId),
            IsFixed = status.IsFixed ?? false,
            Order = status.Order ?? -1,
            PartRoleId = partRoleId == Guid.Empty ? null : partRoleId,
        });
    }

    public async Task<bool> ChangePartTaskStatus(PartTaskStatusModel status)
    {
        bool isPartRoleValid = true;
        Guid.TryParse(status.PartRoleId, out var partRoleId);
        if (status.PartRoleId == "-1")
            isPartRoleValid = false;
        return await repository.ChangePartTaskStatus(new PartTaskStatus
        {
            Id = Guid.Parse(status.Id!),
            Name = status.Name!,
            GlobalStatus = status.GlobalStatus ?? -1,
            PartId = Guid.Parse(status.PartId),
            IsFixed = status.IsFixed ?? false,
            Order = status.Order ?? -1,
            PartRoleId = partRoleId == Guid.Empty && isPartRoleValid ? null : partRoleId,
        });
    }
    public async Task<bool> RemovePartTaskStatus(Guid partId, Guid partTaskStatusId)
    {
        var statuses = await repository.GetPartTaskStatuses(partId);
        if (statuses.Any(status => status.Id == partTaskStatusId && status.IsFixed))
            return false;
        
        var isStatusRemoved = await repository.RemovePartTaskStatus(partId, partTaskStatusId);

        if (isStatusRemoved)
        {
            var order = statuses.FirstOrDefault(status => status.Id == partTaskStatusId)!.Order;
            await pathHelper.CleanTaskPaths(partId, order);
        }
        
        return isStatusRemoved;
    }
    
    public async Task<ICollection<PartTaskStatus>> GetPartTaskStatuses(Guid partId)
    {
        var statuses = await repository.GetPartTaskStatuses(partId);
        return statuses.Select(status => new PartTaskStatus
        {
            Id = status.Id,
            Name = status.Name!,
            IsFixed = status.IsFixed,
            PartId = status.PartId,
            PartRoleId = status.PartRoleId,
            GlobalStatus = status.GlobalStatus,
            Order = status.Order,
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
            TypeId = model.PartTypeId,
            MainPartId = model.MainPartId,
            Parts = model.Parts.Select(ConvertDataModelToLogic).ToList()
        };
    }
}