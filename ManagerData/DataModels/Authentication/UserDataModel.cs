
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels.Authentication;

[Table("Users")]
public class UserDataModel : BaseDataModel
{
    [EmailAddress, Required, MaxLength(40)] 
    public string Email { get; set; } = null!;
    
    [Required] public string PasswordHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public string? MessengerId { get; set; } = null;
    public bool IsAvailable { get; set; } = false;
    
    public IEnumerable<UserRoleDataModel>? Roles { get; set; }
    public IEnumerable<RefreshTokenDataModel>? Tokens { get; set; }
}