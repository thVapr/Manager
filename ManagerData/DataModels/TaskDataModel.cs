
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Tasks")]
public class TaskDataModel : BaseDataModel
{
    public Guid CreatorId { get; init; }
    public Guid? PartId { get; set; }
    
    public int Level { get; set; } = -1;
    public int Status { get; set; } = -1;
    public int Priority { get; set; } = -1;
    public DateTime? StartTime { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ClosedAt { get; set; }

    public PartDataModel? CurrentPart { get; set; }
    public ICollection<TaskMember> TaskMembers { get; set; } = null!;
    public ICollection<MemberDataModel> Members { get; set; } = null!;
}