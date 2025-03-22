
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("PartTasks")]
public class PartTasksDataModel
{
    public Guid PartId { get; set; }
    public Guid TaskId { get; set; }

    public PartDataModel Part { get; set; } = null!;
    public TaskDataModel Task { get; set; } = null!;
}