using System.ComponentModel.DataAnnotations;

namespace ManagerData.DataModels;

public class BackgroundTask
{
    [Key] public Guid Id { get; set; }
    public Guid? PartId { get; set; }
    public Guid TaskId { get; set; }
    public Guid MemberId { get; set; }
    public int Type { get; set; }
    public string? Message { get; set; }
    public DateTime Timeline { get; set; }
    
    public virtual PartDataModel? Part { get; set; }
    public virtual TaskDataModel Task { get; set; }
    public virtual MemberDataModel Member { get; set; }
}