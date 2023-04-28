using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IEmployeeRepository
{
    public Task<bool> CreateEmployee(EmployeeDataModel model);
    public Task<EmployeeDataModel> GetEmployee(Guid id);
    public Task<bool> UpdateEmployee(EmployeeDataModel model);
    public Task<bool> DeleteEmployee(Guid id);
}