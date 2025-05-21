namespace ManagerLogic.Models;

public class TaskHistoryModel : BaseModel
{
    public string TaskId { get; set; }
    public int? SourceStatusId { get; set; }
    public int? DestinationStatusId { get; set; }
    public string InitiatorId { get; set; }
    public string? TargetMemberId { get; set; }
    public int ActionType { get; set; }
    
    public MemberModel Initiator { get; set; }
    public MemberModel TargetMember { get; set; }
}