
namespace ManagerLogic.Models
{
    public class RefreshModel
    {
        public required string AccessToken { get; set; } = string.Empty;
        public required string RefreshToken { get; set; } = string.Empty;
    }
}
