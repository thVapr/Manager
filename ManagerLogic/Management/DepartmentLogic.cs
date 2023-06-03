
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class DepartmentLogic : IDepartmentLogic
{
    private readonly IManagementRepository<DepartmentDataModel> _repository;

    public DepartmentLogic(IManagementRepository<DepartmentDataModel> repository)
    {
        _repository = repository;
    }

    public async Task<DepartmentModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        if (entity.Id == Guid.Empty) return new DepartmentModel();

        return new DepartmentModel
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            ManagerId = entity.ManagerId,
        };
    }

    public Task<IEnumerable<DepartmentModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<DepartmentModel>> GetEntitiesById(Guid id)
    {
        var entities = await _repository.GetEntitiesById(id);
        var result = new List<DepartmentModel>();

        if (entities == null) return result;

        result.AddRange(entities.Where(e => e.Id != Guid.Empty).Select(e => new DepartmentModel
        {
            Id = e.Id.ToString(),
            Name = e.Name,
            Description = e.Description,
            ManagerId = e.ManagerId,
        }));

        return result;
    }

    public async Task<bool> CreateEntity(DepartmentModel model)
    {
        var entity = new DepartmentDataModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name!,
            Description = model.Description!,
        };

        return await _repository.CreateEntity(model.CompanyId, entity);
    }

    public async Task<bool> UpdateEntity(DepartmentModel model)
    {
        return await _repository.UpdateEntity(new DepartmentDataModel
        {
            Id = Guid.Parse(model.Id!),
            Name = model.Name!,
            Description = model.Description!,
            ManagerId = model.ManagerId,
        });
    }

    public Task<IEnumerable<DepartmentModel>> GetEntitiesByQuery(string query, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddEmployeeToDepartment(Guid departmentId, Guid employeeId)
    {
        return await _repository.LinkEntities(departmentId, employeeId);
    }

    public async Task<bool> RemoveEmployeeFromDepartment(Guid departmentId, Guid employeeId)
    {
        return await _repository.UnlinkEntities(departmentId, employeeId);
    }
}