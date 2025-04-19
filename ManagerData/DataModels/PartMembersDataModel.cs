
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("PartMembers")]
public class PartMembersDataModel
{
    public Guid PartId { get; set; }
    public Guid MemberId { get; set; }
    public int Privileges { get; set; }

    public PartDataModel Part { get; set; } = null!;
    public MemberDataModel Member { get; set; } = null!;
}