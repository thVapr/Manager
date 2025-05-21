namespace ManagerData.DataModels;

public class MemberTag
{
    public Guid TagId { get; set; }
    public Guid MemberId { get; set; }
    
    public MemberDataModel Member { get; set; }
    public TagDataModel Tag { get; set; }
}