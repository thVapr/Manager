
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class MemberLogic(IMemberRepository repository) : IMemberLogic
{
    public async Task<MemberModel> GetEntityById(Guid id)
    {
        var entity = await repository.GetEntityById(id);

        return new MemberModel
        {
            Id = entity.Id.ToString(),
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Patronymic = entity.Patronymic,
        };
    }

    public Task<ICollection<MemberModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<MemberModel>> GetEntitiesById(Guid id)
    { 
        var members = await repository.GetEntitiesById(id);

        return members!.Select(e => new MemberModel
        {
            Id = e.Id.ToString(),
            FirstName = e.FirstName,
            LastName = e.LastName,
            Patronymic = e.Patronymic,
        }).ToList();
    }

    public async Task<bool> CreateEntity(MemberModel model)
    {
        if (model.Id == null) return false;

        var entity = new MemberDataModel
        {
            Id = Guid.Parse(model.Id),
            FirstName = model.FirstName!,
            LastName = model.LastName!,
            Patronymic = model.Patronymic!,
        };

        return await repository.CreateEntity(entity);
    }

    public Task<bool> UpdateEntity(MemberModel model)
    {
        return repository.UpdateEntity(new MemberDataModel
        {
            Id = Guid.Parse(model.Id!),
            FirstName = model.FirstName!,
            LastName = model.LastName!,
            Patronymic = model.Patronymic!,
        });
    }

    public Task<bool> AddToEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UnlinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<MemberModel>> GetEntitiesByQuery(string query, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<MemberModel>> GetAvailableMembers(Guid? partId)
    {
        if (partId is null || Guid.Empty == partId) 
            return await GetMembersWithoutPart();
        var membersFromData = await repository.GetAvailableMembersFromPart(partId!.Value);
        return membersFromData.Select(member => new MemberModel
        {
            Id = member.Id.ToString(),
            FirstName = member.FirstName,
            LastName = member.LastName,
            Patronymic = member.Patronymic
        }).ToList();
    }
    
    public async Task<ICollection<MemberModel>> GetMembersWithoutPart()
    {
        // TODO: Нужно изменить структуру учитывая уровень части
        var members = await repository.GetMembersWithoutPart();

        return members.Select(v => new MemberModel
        {
            Id = v.Id.ToString(),
            FirstName = v.FirstName,
            LastName = v.LastName,
            Patronymic = v.Patronymic,
        }).ToList();
    }

    public async Task<ICollection<MemberModel>> GetFreeMembersInPart(Guid id)
    {
        // TODO: Сейчас работает некорректно, нужно также фильтровать по наличию низкоуровневых частей
        //       или пересмотреть необходимость данного метода
        var members = await repository.GetMembersFromPart(id);

        return members.Select(v => new MemberModel
        {
            Id = v.Id.ToString(),
            FirstName = v.FirstName,
            LastName = v.LastName,
            Patronymic = v.Patronymic,
        }).ToList();
    }

    public async Task<ICollection<MemberModel>> GetMembersFromPart(Guid id)
    {
        var members = await repository.GetMembersFromPart(id);

        return members.Select(v => new MemberModel
        {
            Id = v.Id.ToString(),
            FirstName = v.FirstName,
            LastName = v.LastName,
            Patronymic = v.Patronymic,
        }).ToList();
    }
}