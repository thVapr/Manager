
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IEmployeeLogic : IManagementLogic<EmployeeModel>
{
    Task<bool> AddEmployeeToDepartment(Guid departmentId, Guid employeeId);
}