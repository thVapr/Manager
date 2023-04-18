
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels.Authentication;

[Table("Tokens")]
public class RefreshTokenDataModel : BaseDataModel
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime ExpireTime { get; set; }

    public Guid UserId { get; set; }
    public UserDataModel User { get; set; } = null!;
}