
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IEmployeeLogic : IManagementLogic<EmployeeModel>
{
    Task<IEnumerable<EmployeeModel>> GetEmployeesWithoutProjectByDepartmentId(Guid id);
    Task<IEnumerable<EmployeeModel>> GetEmployeesWithoutDepartment();

    Task<IEnumerable<EmployeeModel>> GetEmployeesFromProject(Guid id);
}