
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IDepartmentLogic : IManagementLogic<DepartmentModel>
{
    Task<bool> AddEmployeeToDepartment(Guid departmentId, Guid employeeId);
    Task<bool> RemoveEmployeeFromDepartment(Guid departmentId, Guid employeeId);
}