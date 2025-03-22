
namespace ManagerData.DataModels;

public class WorkspacePartsDataModel
{
    public Guid WorkspaceId { get; set; }
    public Guid PartId { get; set; }

    public WorkspaceDataModel Workspace { get; set; } = null!;
    public PartDataModel Part { get; set; } = null!;
}