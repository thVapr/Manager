
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Tasks")]
public class TaskDataModel : BaseDataModel
{
    public Guid CreatorId { get; init; }
    public Guid? PartId { get; set; }
    public Guid? MemberId { get; set; }
    
    public int Level { get; set; } = 0;
    public int Status { get; set; } = 0;
    public int Priority { get; set; } = 0;

    public MemberTasksDataModel MemberTasks { get; set; } = null!;
    public PartTasksDataModel PartTasks { get; set; } = null!;
}