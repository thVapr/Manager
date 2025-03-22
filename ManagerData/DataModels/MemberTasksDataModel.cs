
namespace ManagerData.DataModels;

public class MemberTasksDataModel
{
    public Guid MemberId { get; set; }
    public Guid TaskId { get; set; }

    public MemberDataModel Member { get; set; } = null!;
    public TaskDataModel Task { get; set; } = null!;
}