using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Parts")]
public class PartDataModel: BaseDataModel
{
    public int Level { get; set; } = -1;
    public int TypeId { get; set; } = 0;

    public IEnumerable<PartDataModel> Parts { get; set; } = new List<PartDataModel>();
    public PartType? PartType { get; set; }
    public Guid? MainPartId { get; set; }
    public PartDataModel? MainPart { get; set; }
    public IEnumerable<TaskDataModel> Tasks { get; set; } = new List<TaskDataModel>();
    public IEnumerable<MemberDataModel> Members { get; set; } = new List<MemberDataModel>();
    public IEnumerable<PartMemberDataModel> PartMembers { get; set; } = new List<PartMemberDataModel>();
    //public IEnumerable<PartTasksDataModel> PartTasks { get; set; } = new List<PartTasksDataModel>();
}