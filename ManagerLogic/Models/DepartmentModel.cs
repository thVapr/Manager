
namespace ManagerLogic.Models;

public class DepartmentModel : BaseModel
{
    public Guid CompanyId { get; set; }
    public IEnumerable<ProjectModel>? Projects { get; set; }
}