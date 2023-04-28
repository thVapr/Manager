using ManagerData.DataModels;

namespace ManagerData.Management;

public interface IDepartment
{
    public Task<bool> CreateDepartment(EmployeeDataModel model);
    public Task<EmployeeDataModel> GetDepartment(Guid id);
    public Task<bool> UpdateDepartment(EmployeeDataModel model);
    public Task<bool> DeleteDepartment(Guid id);
}