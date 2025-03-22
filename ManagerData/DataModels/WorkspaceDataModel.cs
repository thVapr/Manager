
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Workspaces")]
public class WorkspaceDataModel : BaseDataModel
{
    public Guid? ManagerId { get; set; }

    public IEnumerable<WorkspacePartsDataModel>? WorkspaceParts { get; set; }
    public IEnumerable<MemberDataModel>? WorkspaceMember { get; set; }
}