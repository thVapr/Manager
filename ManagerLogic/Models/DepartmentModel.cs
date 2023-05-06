
namespace ManagerLogic.Models;

public class DepartmentModel : BaseModel
{
    public IEnumerable<ProjectModel> Projects { get; set; }
}