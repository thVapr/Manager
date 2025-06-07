using ManagerData.Constants;

namespace ManagerData.DataModels;

public class TaskHistory : BaseDataModel
{
    public Guid TaskId { get; set; }
    public int? SourceStatusId { get; set; }
    public int? DestinationStatusId { get; set; }
    public Guid InitiatorId { get; set; }
    public Guid? TargetMemberId { get; set; }
    public TaskActionType ActionType { get; set; }
    
    public TaskDataModel Task { get; set; }
    public MemberDataModel Initiator { get; set; }
    public MemberDataModel TargetMember { get; set; }
}