using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IProjectRepository
{
    public Task<ProjectDataModel> GetProject(string id);
    public Task<bool> CreateProject(ProjectDataModel model);
    public Task<bool> UpdateProject(ProjectDataModel model);
    public Task<bool> DeleteProject(string id);
}