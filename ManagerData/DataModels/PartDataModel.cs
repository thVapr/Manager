using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Parts")]
public class PartDataModel: BaseDataModel
{
    public Guid? ManagerId { get; set; }
    public int Level { get; set; } = 0;

    public WorkspacePartsDataModel? WorkspaceParts { get; set; }
    public IEnumerable<PartDataModel>? Subparts { get; set; }
    public IEnumerable<PartTasksDataModel>? PartTasks { get; set; }
    public IEnumerable<PartMembersDataModel>? PartMembers { get; set; }
}