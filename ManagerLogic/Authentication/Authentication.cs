

using System.IdentityModel.Tokens.Jwt;
using ManagerData.Authentication;
using ManagerData.DataModels;
using System.Security.Claims;
using ManagerLogic.Models;
using Microsoft.IdentityModel.Tokens;

namespace ManagerLogic.Authentication;

public class Authentication : IAuthentication
{
    private readonly IEncrypt _encrypt;
    private readonly IAuthenticationRepository _authenticationData;
    private readonly IJwtCreator _jwtCreator;

    public Authentication(IEncrypt encrypt, IAuthenticationRepository authenticationData, IJwtCreator jwtCreator)
    {
        _encrypt = encrypt;
        _authenticationData = authenticationData;
        _jwtCreator = jwtCreator;
    }

    public async Task<Tuple<string,string>> Authenticate(LoginModel user)
    {
        var qUser = await _authenticationData.GetUser(user.Email!);
        var passwordHash = _encrypt.HashPassword(user.Password!, qUser.Salt);

        if (qUser.Email == string.Empty ||
            qUser.PasswordHash != passwordHash) 
            return new Tuple<string, string>(string.Empty, string.Empty);

        var role = await _authenticationData.GetUserRole(qUser.Email);

        var claims = new List<Claim>
        {
            new (ClaimTypes.Email, qUser.Email),
            new (ClaimTypes.Role, role)
        };

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
            Id = Guid.NewGuid(),
            Token = _jwtCreator.GenerateRefreshToken(),
            ExpireTime = DateTime.UtcNow.AddMinutes(Constants.ExpiryMinutes),
            UserId = qUser.Id,
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
            Id = Guid.NewGuid(),
            Token = _jwtCreator.GenerateRefreshToken(),
            ExpireTime = DateTime.UtcNow.AddMinutes(Constants.ExpiryMinutes),
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
            Id = Guid.NewGuid(),
            Email = user.Email,
            PasswordHash = hashPassword,
            Salt = salt
        };

        await _authenticationData.AddUser(newUser);

        return true;
    }
}