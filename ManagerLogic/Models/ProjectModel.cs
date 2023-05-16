
namespace ManagerLogic.Models;

public class ProjectModel : BaseModel
{
    public string? ManagerId { get; set; }
 
    public Guid DepartmentId { get; set; }
    public IEnumerable<TaskModel>? Tasks { get; set; }
}