
namespace ManagerLogic.Models;

public class TaskModel : BaseModel
{
    public Guid CreatorId { get; set; }
    public Guid? PartId { get; set; }

    public int Level { get; set; }
    public int Status { get; set; }
    public int Priority { get; set; }
    public Guid? PartRoleId { get; set; }
    public string? Path { get; set; }
    
    public DateTime? StartTime { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ClosedAt { get; set; }
    
    public bool? IsAvailable { get; set; }
}