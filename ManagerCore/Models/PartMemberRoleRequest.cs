using ManagerLogic.Models;

namespace ManagerCore.Models;

public class PartMemberRoleRequest : IPartAllocationModel
{
    public string PartId { get; set; }
    public string MemberId { get; set; }
    public string RoleId { get; set; }
}