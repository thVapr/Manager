
namespace ManagerLogic.Models;

public class ProjectModel : BaseModel
{
    public Guid DepartmentId { get; set; }
    public IEnumerable<TaskModel>? Tasks { get; set; }
}