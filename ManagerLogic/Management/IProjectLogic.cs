using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IProjectLogic : IManagementLogic<ProjectModel>
{
    Task<bool> AddEmployeeToProject(Guid projectId, Guid employeeId);
    Task<bool> RemoveEmployeeFromProject(Guid projectId, Guid employeeId);
}