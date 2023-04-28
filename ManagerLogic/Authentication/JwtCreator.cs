
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManagerLogic.Authentication;

public class JwtCreator : IJwtCreator
{
    public JwtSecurityToken GenerateToken(List<Claim> claims, string issuer, string audience, string secretKey, int expiryMinutes = 60)
    {
        var signCredentials = CreateSigningCredentials(secretKey);

        return new JwtSecurityToken
        (
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: signCredentials
        );
    }

    public string GenerateRefreshToken()
    {
        var guid = Guid.NewGuid();
        return guid.ToString();
    }

    public string GetEmailFromToken(string? token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenData = tokenHandler.ReadJwtToken(token);

        return tokenData.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value;
    }

    public static SigningCredentials CreateSigningCredentials(string secretKey)
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
}