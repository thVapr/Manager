using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Members")]
public class MemberDataModel
{
    [Key] public Guid Id { get; init; }
    [Required] public string LastName { get; set; } = null!;
    [Required] public string FirstName { get; set; } = null!;
    [Required] public string Patronymic { get; set; } = null!;

    public PartMembersDataModel? DepartmentEmployees { get; set; }
    public IEnumerable<MemberTasksDataModel>? EmployeeTasks { get; set; }
}