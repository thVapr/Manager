namespace ManagerLogic.Models;

public class TaskMessageModel
{
    public string? Id { get; set; }
    public string Message { get; set; }
    public string? CreatorId { get; set; }
    public string? TaskId { get; set; }
    public DateTime? CreatedAt { get; set; } 

    public MemberModel? Creator { get; set; }
}