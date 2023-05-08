
namespace ManagerData.DataModels;

public class EmployeeTasksDataModel
{
    public Guid EmployeeId { get; set; }
    public Guid TaskId { get; set; }

    public EmployeeDataModel Employee { get; set; } = null!;
    public TaskDataModel Task { get; set; } = null!;
}