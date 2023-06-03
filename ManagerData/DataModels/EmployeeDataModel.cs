using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Employees")]
public class EmployeeDataModel
{
    [Key] public Guid Id { get; init; }
    [Required] public string LastName { get; set; } = null!;
    [Required] public string FirstName { get; set; } = null!;
    [Required] public string Patronymic { get; set; } = null!;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public ProjectEmployeesDataModel? ProjectEmployees { get; set; }
    public DepartmentEmployeesDataModel? DepartmentEmployees { get; set; }
    public IEnumerable<EmployeeTasksDataModel>? EmployeeTasks { get; set; }
}