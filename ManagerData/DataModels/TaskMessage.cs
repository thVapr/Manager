using System.ComponentModel.DataAnnotations;

namespace ManagerData.DataModels;

public class TaskMessage
{
    [Key] public Guid Id { get; set; }
    public string Message { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TaskId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public MemberDataModel Creator { get; set; }
    public TaskDataModel Task { get; set; }
}