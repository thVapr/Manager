using System.ComponentModel.DataAnnotations;

namespace ManagerData.DataModels;

public class TaskHistory
{
    [Key] public Guid Id { get; init; }
    public Guid TaskId { get; set; }
    public int SourceStatusId { get; set; }
    public int DestinationStatusId { get; set; }
    public Guid InitiatorId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public TaskDataModel Task { get; set; }
    public MemberDataModel Initiator { get; set; }
}