using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Employees")]
public class EmployeeDataModel : BaseDataModel
{
    public EmployeeDataModel()
    {

    }

    public EmployeeDataModel(IEnumerable<ProjectDataModel>? project, IEnumerable<TaskDataModel>? task)
    {
        Project = project;
        Task = task;
    }

    public int UserId { get; set; }
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string Patronymic { get; set; } = null!;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public IEnumerable<ProjectDataModel>? Project { get; set; }
    public IEnumerable<TaskDataModel>? Task { get; set; }
}