
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class PartLogic(IManagementRepository<PartDataModel> repository) : IPartLogic
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
            TypeId = model.TypeId!,
        };
        
        if (model.MasterId.HasValue)
            return await repository.CreateEntity((Guid)model.MasterId, entity);
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

    public Task<bool> AddToEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
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

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddEmployeeToDepartment(Guid departmentId, Guid employeeId)
    {
        return await repository.AddToEntity(departmentId, employeeId);
    }

    public async Task<bool> RemoveEmployeeFromDepartment(Guid departmentId, Guid employeeId)
    {
        return await repository.RemoveFromEntity(departmentId, employeeId);
    }
}