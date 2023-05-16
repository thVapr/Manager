
namespace ManagerLogic.Models;

public class DepartmentModel : BaseModel
{
    public Guid? ManagerId { get; set; }
    public Guid CompanyId { get; set; }
    public IEnumerable<ProjectModel>? Projects { get; set; }
}