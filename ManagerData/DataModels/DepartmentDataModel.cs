
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Department")]
public class DepartmentDataModel: BaseDataModel
{
    [Required] public string Name { get; set; } = null!;
    [Required] public string Description { get; set; } = null!;

    public CompanyDepartmentsDataModel? CompanyDepartments { get; set; }
    public IEnumerable<DepartmentProjectsDataModel>? DepartmentProjects { get; set; }
    public IEnumerable<EmployeeLinksDataModel>? EmployeeLinks { get; set; }
}