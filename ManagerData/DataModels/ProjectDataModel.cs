
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Projects")]
public class ProjectDataModel : BaseDataModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public Guid? ManagerId { get; set; }

    public DepartmentProjectsDataModel? DepartmentProjects { get; set; }
    public IEnumerable<ProjectEmployeesDataModel>? ProjectEmployees { get; set; }
    public IEnumerable<ProjectTasksDataModel>? ProjectTasks { get; set; }
}