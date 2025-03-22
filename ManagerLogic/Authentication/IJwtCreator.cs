
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ManagerLogic.Authentication;

public interface IJwtCreator
{
    JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, string issuer, string audience, string secretKey, int expiryMinutes);
    string GenerateRefreshToken();
    string GetEmailFromToken(string? token);
}