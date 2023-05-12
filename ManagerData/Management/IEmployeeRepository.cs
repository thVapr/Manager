
using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IEmployeeRepository : IManagementRepository<EmployeeDataModel>
{
    Task<IEnumerable<EmployeeDataModel>> GetEmployeesWithoutProjectsByDepartmentId(Guid id);
    Task<IEnumerable<EmployeeDataModel>> GetEmployeesWithoutDepartments();
    Task<IEnumerable<EmployeeDataModel>> GetEmployeesFromProject(Guid id);
}