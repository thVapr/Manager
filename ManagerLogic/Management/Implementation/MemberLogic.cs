using ManagerData.Authentication;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management.Implementation;

public class MemberLogic(IMemberRepository repository, IAuthenticationRepository authenticationRepository) : IMemberLogic
{
    public async Task<MemberModel> GetById(Guid id)
    {
        var entity = await repository.GetById(id);

        return new MemberModel
        {
            Id = entity.Id.ToString(),
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Patronymic = entity.Patronymic,
        };
    }

    public Task<ICollection<MemberModel>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<MemberModel>> GetManyById(Guid id)
    { 
        var members = await repository.GetManyById(id);

        return members!.Select(e => new MemberModel
        {
            Id = e.Id.ToString(),
            FirstName = e.FirstName,
            LastName = e.LastName,
            Patronymic = e.Patronymic,
        }).ToList();
    }

    public async Task<bool> Create(MemberModel model)
    {
        if (model.Id == null) return false;

        var entity = new MemberDataModel
        {
            Id = Guid.Parse(model.Id),
            FirstName = model.FirstName!,
            LastName = model.LastName!,
            Patronymic = model.Patronymic!,
        };

        if (model.MessengerId != null)
        {
            var user = await authenticationRepository.GetUserById(Guid.Parse(model.Id));
            if (user.Id != Guid.Empty)
            {
                user.MessengerId = model.MessengerId;
                await authenticationRepository.UpdateUser(user);
            }
        }

        return await repository.Create(entity);
    }

    public async Task<bool> Update(MemberModel model)
    {
        if (model.MessengerId != null)
        {
            var user = await authenticationRepository.GetUserById(Guid.Parse(model.Id!));
            if (user.Id != Guid.Empty)
            {
                user.MessengerId = model.MessengerId;
                await authenticationRepository.UpdateUser(user);
            }
        }
        
        return await repository.Update(new MemberDataModel
        {
            Id = Guid.Parse(model.Id!),
            
            FirstName = model.FirstName!,
            LastName = model.LastName!,
            Patronymic = model.Patronymic!,
        });
    }

    public Task<bool> AddTo(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveFrom(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddLink(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveLink(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<MemberModel>> GetByQuery(string query, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(Guid id)
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