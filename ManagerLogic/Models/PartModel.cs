
namespace ManagerLogic.Models;

public class PartModel : BaseModel
{
    public Guid? ManagerId { get; set; }
    public Guid CompanyId { get; set; }
    public IEnumerable<PartModel>? Parts { get; set; }
}