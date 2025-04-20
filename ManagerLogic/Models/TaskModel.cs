
namespace ManagerLogic.Models;

public class TaskModel : BaseModel
{
    public Guid CreatorId { get; set; }
    public Guid? PartId { get; set; }
    public Guid? MemberId { get; set; }

    public int Level { get; set; }
    public int Status { get; set; }
    public int Priority { get; set; }
}