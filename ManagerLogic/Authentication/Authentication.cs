
using ManagerLogic.Models;
using System.Security.Claims;
using ManagerData.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ManagerData.DataModels.Authentication;

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
            new (PublicConstants.Id, userFromQuery.Id.ToString()),
            new (PublicConstants.Email, userFromQuery.Email),
            new (PublicConstants.Role, role)
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
            Token = _jwtCreator.GenerateRefreshToken(),
            ExpireTime = DateTime.UtcNow.AddDays(Constants.ExpiryMinutes),
            UserId = userFromQuery.Id,
        };

        await _authenticationData.AddToken(tokenModel);

        return new Tuple<string, string>(tokenHandler.WriteToken(token),tokenModel.Token);
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

        var claims = new List<Claim>
        {
            new (PublicConstants.Id, user.Id.ToString()),
            new (PublicConstants.Email, user.Email),
            new (PublicConstants.Role, role)
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

    public Task<UserDataModel> GetUser(string email)
    {
        return _authenticationData.GetUser(email);
    }

    public async Task<IEnumerable<string>> GetAdminIds()
    {
        var adminIds = await _authenticationData.GetAdminIds();

        return adminIds.Select(x => x.ToString());
    }

    public async Task<bool> Logout(string token)
    {
        var qToken = await _authenticationData.GetToken(token);

        return qToken != null && await _authenticationData.DeleteToken(token);
    }

    public async Task<bool> CreateUser(LoginModel user)
    {
        var userCheck = await _authenticationData.GetUser(user.Email);

        if (!string.IsNullOrEmpty(userCheck.Email)) return false;

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

    public Task<bool> RemoveUser(Guid id)
    {
        return _authenticationData.DeleteUser(id);
    }
}