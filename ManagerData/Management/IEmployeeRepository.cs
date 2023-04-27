using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IEmployeeRepository
{
    public Task<EmployeeDataModel> GetEmployee(string id);
    public Task<bool> CreateEmployee(EmployeeDataModel model);
    public Task<bool> UpdateEmployee(EmployeeDataModel model);
    public Task<bool> DeleteEmployee(string id);
}