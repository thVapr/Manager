
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
    public Guid? PartRoleId { get; set; }
    public Guid? TaskTypeId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string Path { get; set; } = string.Empty;

    public PartDataModel? CurrentPart { get; set; }
    public PartRole? PartRole { get; set; }
    public PartTaskType? TaskType { get; set; }
    public ICollection<TaskMember> TaskMembers { get; set; } = null!;
    public ICollection<MemberDataModel> Members { get; set; } = null!;
}