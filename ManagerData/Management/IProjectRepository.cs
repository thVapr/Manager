using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IProjectRepository
{
    public Task<bool> CreateProject(ProjectDataModel model);
    public Task<ProjectDataModel> GetProject(Guid id);
    public Task<bool> UpdateProject(ProjectDataModel model);
    public Task<bool> DeleteProject(Guid id);
}