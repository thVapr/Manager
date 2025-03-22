
using ManagerLogic.Models;
using ManagerData.DataModels;
using ManagerData.Management;

namespace ManagerLogic.Management;

public class WorkspaceLogic : IManagementLogic<WorkspaceModel>
{
    private readonly IManagementRepository<WorkspaceDataModel> _repository;

    public WorkspaceLogic(IManagementRepository<WorkspaceDataModel> repository)
    {
        _repository = repository;
    }

    public async Task<WorkspaceModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        if (entity.Id == Guid.Empty) return new WorkspaceModel();

        return new WorkspaceModel
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<IEnumerable<WorkspaceModel>> GetEntities()
    {
        var entities = await _repository.GetEntities();

        if (entities == null) return Enumerable.Empty<WorkspaceModel>();

        return entities
            .Where(e => e.Id != Guid.Empty)
            .Select(e => new WorkspaceModel
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                Description = e.Description
            })
            .ToList();
    }

    public Task<IEnumerable<WorkspaceModel>> GetEntitiesById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CreateEntity(WorkspaceModel model)
    {
        var entity = new WorkspaceDataModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name!,
            Description = model.Description!,
        };

        return await _repository.CreateEntity(entity);
    }

    public Task<bool> UpdateEntity(WorkspaceModel model)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<WorkspaceModel>> GetEntitiesByQuery(string query, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }
}