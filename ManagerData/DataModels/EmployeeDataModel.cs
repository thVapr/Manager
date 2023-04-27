using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;

namespace ManagerData.DataModels;

[Table("Employees")]
public class EmployeeDataModel : BaseDataModel
{
    public int UserId { get; set; }

    [Required] public string LastName { get; set; } = null!;
    [Required] public string FirstName { get; set; } = null!;
    [Required] public string Patronymic { get; set; } = null!;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public IEnumerable<EmployeeLinksDataModel>? EmployeeLink { get; set; }
}