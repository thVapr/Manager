using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Members")]
public class MemberDataModel
{
    [Key] public Guid Id { get; set; }
    [Required, MaxLength(111)] public string LastName { get; set; } = null!;
    [Required, MaxLength(111)] public string FirstName { get; set; } = null!;
    [Required, MaxLength(111)] public string Patronymic { get; set; } = null!;
    
    public IEnumerable<PartDataModel> Parts { get; set; } = new List<PartDataModel>();
    public IEnumerable<PartMemberDataModel> PartLinks { get; set; } = new List<PartMemberDataModel>();
    public IEnumerable<TaskDataModel>  Tasks { get; set; } = new List<TaskDataModel>();
    public IEnumerable<TaskMember> MemberTasks { get; set; } = new List<TaskMember>();
}