
namespace ManagerData.DataModels;

public class ProjectEmployeesDataModel
{
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }

    public ProjectDataModel Project { get; set; } = null!;
    public EmployeeDataModel Employee { get; set; } = null!;
}