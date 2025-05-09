namespace ManagerData.DataModels;

public class PartMemberRole
{
    public Guid PartRoleId { get; set; }
    public Guid MemberId { get; set; }
    public Guid PartId { get; set; }
    
    public MemberDataModel Member { get; set; } = null!;
    public PartDataModel Part { get; set; } = null!;
}