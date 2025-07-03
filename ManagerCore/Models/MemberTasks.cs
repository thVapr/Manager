using ManagerLogic.Models;

namespace ManagerCore.Models;

public class MemberTasks : IPartAllocationModel
{
    public string? MemberId { get; set; }
    public string? TaskId { get; set; }
    public string? PartId { get; set; }
    public int? GroupId { get; set; }
}