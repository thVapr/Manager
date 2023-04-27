using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("EmployeeLinks")]
public class EmployeeLinksDataModel
{
    public Guid EmployeeId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TaskId { get; set; }

    public EmployeeDataModel Employee { get; set; } = null!;
    public DepartmentDataModel Department { get; set; } = null!;
    public ProjectDataModel Project { get; set; } = null!;
    public TaskDataModel Task { get; set; } = null!;
}