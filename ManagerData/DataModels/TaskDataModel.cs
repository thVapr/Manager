
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Tasks")]
public class TaskDataModel : BaseDataModel
{
    public Guid CreatorId { get; init; }
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int Level { get; set; } = 0;
    public int Status { get; set; } = 0;

    public EmployeeTasksDataModel EmployeeTasks { get; set; } = null!;
    public ProjectTasksDataModel ProjectTasks { get; set; } = null!;
}