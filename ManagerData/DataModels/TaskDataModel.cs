
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Tasks")]
public class TaskDataModel : BaseDataModel
{
    public Guid CreatorId { get; init; }
    public Guid? PartId { get; set; }
    
    public int Level { get; set; } = 0;
    public int Status { get; set; } = 0;
    public int Priority { get; set; } = 0;
    public DateTime Deadline { get; set; }
    public DateTime ClosedAt { get; set; }

    public PartDataModel? CurrentPart { get; set; }
    public ICollection<TaskMember> TaskMembers { get; set; } = null!;
    public ICollection<MemberDataModel> Members { get; set; } = null!;
    //public PartTasksDataModel NotificationGroup { get; set; } = null!;
}