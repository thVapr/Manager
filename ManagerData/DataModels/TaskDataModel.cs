
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Tasks")]
public class TaskDataModel : BaseDataModel
{
    public TaskDataModel()
    {

    }

    public TaskDataModel(ProjectDataModel? project, EmployeeDataModel? employee)
    {
        Project = project;
        Employee = employee;
    }

    public Guid CreatorId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int Level { get; set; } = 0;
    
    public ProjectDataModel? Project { get; set; }
    public EmployeeDataModel? Employee { get; set; }

}