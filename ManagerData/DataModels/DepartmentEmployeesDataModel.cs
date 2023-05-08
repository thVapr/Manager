
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("DepartmentEmployees")]
public class DepartmentEmployeesDataModel
{
    public Guid DepartmentId { get; set; }
    public Guid EmployeeId { get; set; }

    public DepartmentDataModel Department { get; set; } = null!;
    public EmployeeDataModel Employee { get; set; } = null!;
}