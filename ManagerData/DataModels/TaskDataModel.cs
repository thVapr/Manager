
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Tasks")]
public class TaskDataModel : BaseDataModel
{
    public Guid CreatorId { get; init; }
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int Level { get; set; } = 0;

    public EmployeeLinksDataModel EmployeeLinks { get; set; } = null!;
    public ProjectTasksDataModel ProjectTasks { get; set; } = null!;
}