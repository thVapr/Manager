
namespace ManagerLogic.Models;

public class EmployeeModel
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }

    public IEnumerable<DepartmentModel> Companies { get; set; }
}