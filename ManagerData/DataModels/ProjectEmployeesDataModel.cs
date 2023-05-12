
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("ProjectEmployees")]
public class ProjectEmployeesDataModel
{
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }

    public ProjectDataModel Project { get; set; } = null!;
    public EmployeeDataModel Employee { get; set; } = null!;
}