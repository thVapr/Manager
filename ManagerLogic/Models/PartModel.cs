
namespace ManagerLogic.Models;

public class PartModel : BaseModel
{
    public Guid? ManagerId { get; set; }
    public Guid WorkspaceId { get; set; }
    public IEnumerable<PartModel>? Parts { get; set; }
}