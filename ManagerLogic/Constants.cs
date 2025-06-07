
using ManagerData.Constants;

namespace ManagerLogic;

public static class Constants
{
    public static string GetSecureKey()
    {
        var secretProvider = new SecretProvider();
        return secretProvider.GetTokenSecretKey()!;
    }
    public static readonly int ExpiryMinutes = 60;
    public static readonly string Issuer = "http:localhost:6732";
    public static readonly string Audience = "http:localhost:6732";
}