
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Roles")]
public class RoleDataModel : BaseDataModel
{
    [Required, MaxLength(40)] public string Name { get; set; } = null!;

    public IEnumerable<UserRoleDataModel>? Users { get; set; }
}