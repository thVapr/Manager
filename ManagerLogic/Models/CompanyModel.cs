
namespace ManagerLogic.Models;

public class CompanyModel : BaseModel
{
    public IEnumerable<DepartmentModel> Departments { get; set; }
}