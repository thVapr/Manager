using ManagerLogic.Models;

namespace ManagerCore.Models;

public class AddToPartModel : IPartAllocationModel
{
    public string? EntityId { get; set; }
    public string PartId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}