using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ManagerData.Authentication;
using ManagerData.Constants;
using ManagerData.DataModels.Authentication;
using ManagerLogic.Models;
using Microsoft.IdentityModel.Tokens;

namespace ManagerLogic.Authentication;

public class Authentication(IEncrypt encrypt, IAuthenticationRepository authenticationData, IJwtCreator jwtCreator)
    : IAuthentication
{
    public async Task<Tuple<string,string>> Authenticate(LoginModel user)
    {
        var userFromQuery = await authenticationData.GetUser(user.Email!);
        if (string.IsNullOrEmpty(userFromQuery.Salt))
            return new Tuple<string, string>(string.Empty, string.Empty);
        var passwordHash = encrypt.HashPassword(user.Password!, userFromQuery.Salt);

        if (userFromQuery.Email == string.Empty ||
            userFromQuery.PasswordHash != passwordHash) 
            return new Tuple<string, string>(string.Empty, string.Empty);

        var role = await authenticationData.GetUserRole(userFromQuery.Email);

        var claims = new List<Claim>
        {
            new (PublicConstants.Id, userFromQuery.Id.ToString()),
            new (PublicConstants.Email, userFromQuery.Email),
            new (PublicConstants.Role, role)
        };

        var token = jwtCreator.GenerateToken
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
            Token = jwtCreator.GenerateRefreshToken(),
            ExpireTime = DateTime.UtcNow.AddDays(Constants.ExpiryMinutes),
            UserId = userFromQuery.Id,
        };

        await authenticationData.AddToken(tokenModel);

        return new Tuple<string, string>(tokenHandler.WriteToken(token),tokenModel.Token);
    }

    public async Task<Tuple<string, string>> UpdateToken(RefreshModel model)
    {
        if (model is null) 
            throw new SecurityTokenException();
        
        var existingToken = await authenticationData.GetToken(model.RefreshToken!);
        var email = jwtCreator.GetEmailFromToken(model.AccessToken);
        var user = await authenticationData.GetUser(email!);
        var role = await authenticationData.GetUserRole(email!);

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

        var newAccessToken = jwtCreator.GenerateToken
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
            Token = jwtCreator.GenerateRefreshToken(),
            ExpireTime = DateTime.UtcNow.AddDays(Constants.ExpiryMinutes),
            UserId = user.Id,
        };

        await authenticationData.UpdateToken(tokenModel, model.RefreshToken!);

        return new Tuple<string, string>(tokenHandler.WriteToken(newAccessToken), tokenModel.Token);
    }

    public Task<UserDataModel> GetUser(string email)
    {
        return authenticationData.GetUser(email);
    }

    public async Task<UserDataModel> GetUserById(Guid id)
    {
        return await authenticationData.GetUserById(id);
    }

    public async Task<UserDataModel> GetUserByMessengerId(string id)
    {
        return (await authenticationData.GetUsers())
            .FirstOrDefault(u => u.MessengerId == id) ?? new UserDataModel();
    }

    public async Task<IEnumerable<string>> GetAdminIds()
    {
        var adminIds = await authenticationData.GetAdminIds();

        return adminIds.Select(x => x.ToString());
    }

    public async Task<ICollection<string>> GetAvailableUserIds()
    {
        var users = await authenticationData.GetUsers();
        var adminIds = await GetAdminIds();
        return users.Where(user => user.IsAvailable && !adminIds
                .Contains(user.Id.ToString()))
            .Select(user => user.Id.ToString())
            .ToList();
    }

    public async Task<bool> Logout(string token)
    {
        var qToken = await authenticationData.GetToken(token);

        return qToken != null && await authenticationData.DeleteToken(token);
    }

    public async Task<bool> CreateUser(LoginModel user)
    {
        var userCheck = await authenticationData.GetUser(user.Email!);

        if (!string.IsNullOrEmpty(userCheck.Email)) return false;

        var salt = Guid.NewGuid().ToString();
        var hashPassword = encrypt.HashPassword(user.Password!, salt);

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

        await authenticationData.AddUser(newUser);

        return true;
    }

    public async Task<bool> UpdateUser(UserDataModel user)
    {
        return await authenticationData.UpdateUser(user);
    }

    public async Task<bool> RemoveUser(Guid id)
    {
        return await authenticationData.DeleteUser(id);
    }
}