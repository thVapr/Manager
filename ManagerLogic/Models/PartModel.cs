
namespace ManagerLogic.Models;

public class PartModel : BaseModel
{
    public int Level { get; set; } = 0;
    public int TypeId { get; set; } = 0;
    public Guid? MainPartId { get; set; }
    public IEnumerable<PartModel>? Parts { get; set; }
}