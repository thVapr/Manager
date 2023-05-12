
namespace ManagerLogic.Models;

public class TaskModel : BaseModel
{
    public Guid EmployeeId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid CreatorId { get; set; }

    public int Level { get; set; }
}