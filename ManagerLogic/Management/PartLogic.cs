
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class PartLogic : IPartLogic
{
    private readonly IManagementRepository<PartDataModel> _repository;

    public PartLogic(IManagementRepository<PartDataModel> repository)
    {
        _repository = repository;
    }

    public async Task<PartModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        if (entity.Id == Guid.Empty) return new PartModel();

        return new PartModel
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            ManagerId = entity.ManagerId,
        };
    }

    public Task<IEnumerable<PartModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PartModel>> GetEntitiesById(Guid id)
    {
        var entities = await _repository.GetEntitiesById(id);
        var result = new List<PartModel>();

        if (entities == null) return result;

        result.AddRange(entities.Where(e => e.Id != Guid.Empty).Select(e => new PartModel
        {
            Id = e.Id.ToString(),
            Name = e.Name,
            Description = e.Description,
            ManagerId = e.ManagerId,
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
        };

        return await _repository.CreateEntity(model.WorkspaceId, entity);
    }

    public async Task<bool> UpdateEntity(PartModel model)
    {
        return await _repository.UpdateEntity(new PartDataModel
        {
            Id = Guid.Parse(model.Id!),
            Name = model.Name!,
            Description = model.Description!,
            ManagerId = model.ManagerId,
        });
    }

    public Task<IEnumerable<PartModel>> GetEntitiesByQuery(string query, Guid id)
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