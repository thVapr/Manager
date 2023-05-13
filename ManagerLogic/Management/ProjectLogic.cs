
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class ProjectLogic : IProjectLogic
{
    private readonly IManagementRepository<ProjectDataModel> _repository;

    public ProjectLogic(IManagementRepository<ProjectDataModel> repository)
    {
        _repository = repository;
    }
    
    public async Task<ProjectModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        if (entity.Id == Guid.Empty) return new ProjectModel();

        return new ProjectModel
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
        };
    }

    public Task<IEnumerable<ProjectModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProjectModel>> GetEntitiesById(Guid id)
    {
        var entities = await _repository.GetEntitiesById(id);

        if (entities == null) return Enumerable.Empty<ProjectModel>();

        return entities
            .Where(e => e.Id != Guid.Empty)
            .Select(e => new ProjectModel
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                Description = e.Description,
            }).ToList();
    }

    public async Task<bool> CreateEntity(ProjectModel model)
    {
        var entity = new ProjectDataModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description
        };

        return await _repository.CreateEntity(model.DepartmentId, entity);
    }

    public Task<bool> UpdateEntity(ProjectModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddEmployeeToProject(Guid projectId, Guid employeeId)
    {
        return await _repository.LinkEntities(projectId, employeeId);
    }

    public async Task<bool> RemoveEmployeeFromProject(Guid projectId, Guid employeeId)
    {
        return await _repository.UnlinkEntities(projectId, employeeId);
    }
}