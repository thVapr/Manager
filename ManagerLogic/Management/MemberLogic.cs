
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class MemberLogic : IMemberLogic
{
    private readonly IMemberRepository _repository;

    public MemberLogic(IMemberRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<EmployeeModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        return new EmployeeModel
        {
            Id = entity.Id.ToString(),
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Patronymic = entity.Patronymic,
        };
    }

    public Task<IEnumerable<EmployeeModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<EmployeeModel>> GetEntitiesById(Guid id)
    { 
        var employees = await _repository.GetEntitiesById(id);

        return employees!.Select(e => new EmployeeModel
        {
            Id = e.Id.ToString(),
            FirstName = e.FirstName,
            LastName = e.LastName,
            Patronymic = e.Patronymic,
        });
    }

    public async Task<bool> CreateEntity(EmployeeModel model)
    {
        if (model.Id == null) return false;

        var entity = new MemberDataModel
        {
            Id = Guid.Parse(model.Id),
            FirstName = model.FirstName!,
            LastName = model.LastName!,
            Patronymic = model.Patronymic!,
        };

        return await _repository.CreateEntity(entity);
    }

    public Task<bool> UpdateEntity(EmployeeModel model)
    {
        return _repository.UpdateEntity(new MemberDataModel
        {
            Id = Guid.Parse(model.Id!),
            FirstName = model.FirstName!,
            LastName = model.LastName!,
            Patronymic = model.Patronymic!,
        });
    }

    public Task<IEnumerable<EmployeeModel>> GetEntitiesByQuery(string query, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<EmployeeModel>> GetMembersWithoutPart()
    {
        // TODO: Нужно изменить структуру учитывая уровень части
        var employees = await _repository.GetMembersWithoutPart(0);

        return employees.Select(v => new EmployeeModel
        {
            Id = v.Id.ToString(),
            FirstName = v.FirstName,
            LastName = v.LastName,
            Patronymic = v.Patronymic,
        }).ToList();
    }

    public async Task<IEnumerable<EmployeeModel>> GetFreeMembersInPart(Guid id)
    {
        // TODO: Сейчас работает некорректно, нужно также фильтровать по наличию низкоуровневых частей
        //       или пересмотреть необходимость данного метода
        var employees = await _repository.GetMembersFromPart(id);

        return employees.Select(v => new EmployeeModel
        {
            Id = v.Id.ToString(),
            FirstName = v.FirstName,
            LastName = v.LastName,
            Patronymic = v.Patronymic,
        }).ToList();
    }

    public async Task<IEnumerable<EmployeeModel>> GetMembersFromPart(Guid id)
    {
        // TODO: Нужно подумать о том, какие участники должны быть возвращены,
        //       в текущей части или включая все нижестоящие
        var members = await _repository.GetMembersFromPart(id);

        return members.Select(v => new EmployeeModel
        {
            Id = v.Id.ToString(),
            FirstName = v.FirstName,
            LastName = v.LastName,
            Patronymic = v.Patronymic,
        }).ToList();
    }
}