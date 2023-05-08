
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

        var department = new ProjectModel
        {
            Name = entity.Name,
            Description = entity.Description,
        };
        
        return department;
    }

    public async Task<bool> CreateEntity(ProjectModel model)
    {
        var entity = new ProjectDataModel
        {
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
}