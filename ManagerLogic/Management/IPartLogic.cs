
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IPartLogic : IManagementLogic<PartModel>
{
    Task<bool> AddEmployeeToDepartment(Guid departmentId, Guid employeeId);
    Task<bool> RemoveEmployeeFromDepartment(Guid departmentId, Guid employeeId);
}