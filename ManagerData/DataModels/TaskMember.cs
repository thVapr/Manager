namespace ManagerData.DataModels;

public class TaskMember
{
    public Guid MemberId { get; set; }
    public Guid TaskId { get; set; }
    public int ParticipationPurpose { get; set; }

    public MemberDataModel Member { get; set; } = null!;
    public TaskDataModel Task { get; set; } = null!;
}