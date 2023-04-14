using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("UserRoles")]
public class UserRoleDataModel
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public UserDataModel User { get; set; } = null!;
    public RoleDataModel Role { get; set; } = null!;
}

