
namespace ManagerData.DataModels;

public class DepartmentProjectsDataModel
{
    public Guid DepartmentId { get; set; }
    public Guid ProjectId { get; set; }

    public DepartmentDataModel Department { get; set; } = null!;
    public ProjectDataModel Project { get; set; } = null!;
}