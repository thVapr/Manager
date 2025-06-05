using ManagerData.Contexts;
using ManagerData.Constants;
using ManagerData.DataModels.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagerData.Authentication;

public class AuthenticationRepository(ILogger<AuthenticationRepository> logger) : IAuthenticationRepository
{
    public async Task<UserDataModel> GetUser(Guid id)
    {
        await using var database = new AuthenticationDbContext();
        
        try
        {
            return await database.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync() ?? new UserDataModel();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return new UserDataModel();
        }
    }

    public async Task<UserDataModel> GetUser(string email)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            return await database.Users
                .Where(o => o.Email == email)
                .FirstOrDefaultAsync() ?? new UserDataModel();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return new UserDataModel();
        }
    }

    public async Task<UserDataModel> GetUserById(Guid userId)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            return await database.Users
                .FirstOrDefaultAsync(user => user.Id == userId) ?? new UserDataModel();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return new UserDataModel();
        }
    }

    public async Task<ICollection<UserDataModel>> GetUsers()
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            return await database.Users
                .ToListAsync() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<string> GetUserRole(string email)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var user = await GetUser(email);
            var userRoles = await database.UserRoles
                .Where(u => u.UserId == user.Id)
                .FirstOrDefaultAsync();
            var role = await database.Roles
                .Where(r => r.Id == userRoles!.RoleId)
                .FirstOrDefaultAsync();

            if (role == null) await SeedRoles();

            return role!.Name;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return string.Empty;
        }
    }

    public async Task<RefreshTokenDataModel?> GetToken(Guid userId)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            return await database.Tokens.FirstOrDefaultAsync(u => u.UserId == userId) ?? new RefreshTokenDataModel();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return new RefreshTokenDataModel();
        }
    }

    public async Task<RefreshTokenDataModel?> GetToken(string token)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            return await database.Tokens.FirstOrDefaultAsync(u => u.Token == token) ?? new RefreshTokenDataModel();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return new RefreshTokenDataModel();
        }
    }

    public async Task<bool> AddToken(RefreshTokenDataModel token)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var existingToken = await database.Tokens.FirstOrDefaultAsync(t => t.UserId == token.UserId);

            if (existingToken != null)
            {
                database.Tokens.Remove(existingToken);
            }

            await database.Tokens.AddAsync(token);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> UpdateToken(RefreshTokenDataModel tokenModel, string token)
    {
        await using var database = new AuthenticationDbContext();

        var existingToken = database.Tokens.FirstOrDefault(t => t.Token == token);

        if (existingToken is null) return false;

        await DeleteToken(token);
        await AddToken(tokenModel);
        
        return true;
    }

    public async Task<bool> DeleteToken(string token)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var existingToken = await database.Tokens.FirstOrDefaultAsync(t => t.Token == token);

            if (existingToken == null) return false;
            
            database.Tokens.Remove(existingToken);
            await database.SaveChangesAsync();
            return true;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<IEnumerable<Guid>> GetAdminIds()
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var roleId = await database.Roles
                .Where(r => r.Name == RoleConstants.Admin)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            return await database.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(r => r.UserId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<bool> AddUser(UserDataModel user)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var existingUser = await database.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null) return false;

            var role = await database.Roles.FirstOrDefaultAsync(r => r.Name == Constants.RoleConstants.Default);

            if (role == null)
            {
                await SeedRoles();
                role = await database.Roles.FirstOrDefaultAsync(r => r.Name == Constants.RoleConstants.Default);
            }

            var userRole = new UserRoleDataModel
            {
                UserId = user.Id,
                RoleId = role!.Id
            };

            database.Users.Add(user);
            database.UserRoles.Add(userRole);
            
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> AddRole(string name)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var role = new RoleDataModel
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            var roles = await database.Roles.FirstOrDefaultAsync(r => r.Name == name);

            if (roles != null) return false;

            await database.Roles.AddAsync(role);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> UpdateUser(UserDataModel model)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var user = await database.Users.FindAsync(model.Id);

            if (user == null) return false;

            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;
            if (!string.IsNullOrEmpty(model.PasswordHash) && !string.IsNullOrEmpty(model.Salt))
            {
                user.PasswordHash = model.PasswordHash;
                user.Salt = model.Salt;
            }
            if (!string.IsNullOrEmpty(model.ChatId))
            {
                user.ChatId = model.ChatId;
            }
            if (!string.IsNullOrEmpty(model.MessengerId) && user.MessengerId != model.MessengerId)
            {
                user.ChatId = "";
                user.MessengerId = model.MessengerId;
            }
            
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var user = await database.Users.FindAsync(id);

            if (user == null) return false;

            database.Users.Remove(user);
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    private async Task SeedRoles()
    {
        await AddRole(RoleConstants.SpaceOwner);
        await AddRole(RoleConstants.Default);
        await AddRole(RoleConstants.Admin);
    }
}