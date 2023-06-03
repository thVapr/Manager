
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels.Authentication;

[Table("Roles")]
public class RoleDataModel : BaseDataModel
{
    public IEnumerable<UserRoleDataModel>? Users { get; set; }
}