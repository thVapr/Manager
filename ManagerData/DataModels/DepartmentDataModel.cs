
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Department")]
public class DepartmentDataModel: BaseDataModel
{
    public Guid? ManagerId { get; set; }

    public CompanyDepartmentsDataModel? CompanyDepartments { get; set; }
    public IEnumerable<DepartmentProjectsDataModel>? DepartmentProjects { get; set; }
    public IEnumerable<DepartmentEmployeesDataModel>? DepartmentEmployees { get; set; }
}