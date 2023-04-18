
using System.IdentityModel.Tokens.Jwt;
using ManagerData.Authentication;
using System.Security.Claims;
using ManagerData.DataModels.Authentication;
using ManagerLogic.Models;
using Microsoft.IdentityModel.Tokens;

namespace ManagerLogic.Authentication;

public class Authentication : IAuthentication {
    private readonly IEncrypt _encrypt;
    private readonly IAuthenticationRepository _authenticationData;
    private readonly IJwtCreator _jwtCreator;

    public Authentication(IEncrypt encrypt, IAuthenticationRepository authenticationData, IJwtCreator jwtCreator)
    {
        _encrypt = encrypt;
        _authenticationData = authenticationData;
        _jwtCreator = jwtCreator;
    }
    //TODO: add server validation
    public async Task<Tuple<string,string>> Authenticate(LoginModel user)
    {
        var userFromQuery = await _authenticationData.GetUser(user.Email!);
        var passwordHash = _encrypt.HashPassword(user.Password!, userFromQuery.Salt);

        if (userFromQuery.Email == string.Empty ||
            userFromQuery.PasswordHash != passwordHash) 
            return new Tuple<string, string>(string.Empty, string.Empty);

        var role = await _authenticationData.GetUserRole(userFromQuery.Email);

        var claims = new List<Claim>
        {
            new (ClaimTypes.Email, userFromQuery.Email),
            new (ClaimTypes.Role, role)
        };
        //TODO: refactor this shit
        var token = _jwtCreator.GenerateToken
        (
            claims,
            Constants.Issuer,
            Constants.Audience,
            Constants.SecureKey,
            Constants.ExpiryMinutes
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenModel = new RefreshTokenDataModel
        {
            Token = _jwtCreator.GenerateRefreshToken(),
            ExpireTime = DateTime.UtcNow.AddDays(Constants.ExpiryMinutes),
            UserId = userFromQuery.Id,
        };

        await _authenticationData.AddToken(tokenModel);

        return new Tuple<string, string>(tokenHandler.WriteToken(token),tokenModel.Token);
        // TODO: add exceptions
    }

    public async Task<Tuple<string, string>> UpdateToken(RefreshModel model)
    {
        if (model is null) 
            throw new SecurityTokenException();
        
        var existingToken = await _authenticationData.GetToken(model.RefreshToken);
        var email = _jwtCreator.GetEmailFromToken(model.AccessToken);
        var user = await _authenticationData.GetUser(email!);
        var role = await _authenticationData.GetUserRole(email!);

        if (user is null ||
            existingToken is null ||
            existingToken.ExpireTime <= DateTime.UtcNow)
            throw new InvalidDataException();

        //TODO:Refactor this
        var claims = new List<Claim>
        {
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.Role, role)
        };

        var newAccessToken = _jwtCreator.GenerateToken
        (
            claims,
            Constants.Issuer,
            Constants.Audience,
            Constants.SecureKey,
            Constants.ExpiryMinutes
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenModel = new RefreshTokenDataModel
        {
            Token = _jwtCreator.GenerateRefreshToken(),
            ExpireTime = DateTime.UtcNow.AddDays(Constants.ExpiryMinutes),
            UserId = user.Id,
        };

        await _authenticationData.UpdateToken(tokenModel, model.RefreshToken);

        return new Tuple<string, string>(tokenHandler.WriteToken(newAccessToken), tokenModel.Token);
    }

    public async Task<bool> Logout(string token)
    {
        var qToken = await _authenticationData.GetToken(token);

        return qToken != null && await _authenticationData.DeleteToken(token);
    }

    public async Task<bool> CreateUser(LoginModel user)
    {
        var userCheck = await _authenticationData.GetUser(user.Email);

        if (!userCheck.Email.IsNullOrEmpty()) return false;

        var salt = Guid.NewGuid().ToString();
        var hashPassword = _encrypt.HashPassword(user.Password, salt);

        if (string.IsNullOrEmpty(hashPassword) ||
            string.IsNullOrEmpty(salt) ||
            string.IsNullOrEmpty(user.Email))
        {
            return false;
        }

        var newUser = new UserDataModel
        {
            Email = user.Email,
            PasswordHash = hashPassword,
            Salt = salt
        };

        await _authenticationData.AddUser(newUser);

        return true;
    }
}