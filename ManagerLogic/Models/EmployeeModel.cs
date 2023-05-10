
namespace ManagerLogic.Models;

public class EmployeeModel
{
    public string? UserId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }

    public Guid? DepartmentId { get; set; }

    public IEnumerable<DepartmentModel>? Departments { get; set; }
}