
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("ProjectTasks")]
public class ProjectTasksDataModel
{
    public Guid ProjectId { get; set; }
    public Guid TaskId { get; set; }

    public ProjectDataModel Project { get; set; } = null!;
    public TaskDataModel Task { get; set; } = null!;
}