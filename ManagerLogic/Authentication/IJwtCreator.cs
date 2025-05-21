
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ManagerLogic.Authentication;

public interface IJwtCreator
{
    JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, string issuer, string audience, string secretKey, int expiryMinutes);
    string GenerateRefreshToken();
    string GetEmailFromToken(string? token);
}