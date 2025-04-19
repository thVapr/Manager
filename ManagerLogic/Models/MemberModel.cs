
namespace ManagerLogic.Models;

public class MemberModel
{
    public string? Id { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }

    public Guid? PartId { get; set; }

    public IEnumerable<PartModel>? Parts { get; set; }
}