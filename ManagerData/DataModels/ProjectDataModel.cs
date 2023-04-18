
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Projects")]
public class ProjectDataModel : BaseDataModel
{
    public ProjectDataModel()
    {

    }

    public ProjectDataModel(IEnumerable<TaskDataModel>? tasks, IEnumerable<EmployeeDataModel>? employees)
    {
        Tasks = tasks;
        Employees = employees;
    }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid TaskId { get; set; }

    public IEnumerable<TaskDataModel>? Tasks { get; set; }
    public IEnumerable<EmployeeDataModel>? Employees { get; set; }

}