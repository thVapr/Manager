
using ManagerData.DataModels;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public interface IMemberLogic : IManagementLogic<EmployeeModel>
{
    Task<IEnumerable<EmployeeModel>> GetMembersWithoutPart();
    Task<IEnumerable<EmployeeModel>> GetFreeMembersInPart(Guid id);

    Task<IEnumerable<EmployeeModel>> GetMembersFromPart(Guid id);
}