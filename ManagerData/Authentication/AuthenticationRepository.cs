
using ManagerData.Contexts;
using ManagerData.DataModels.Authentication;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Authentication;

public class AuthenticationRepository : IAuthenticationRepository, IDisposable
{
    public async Task<UserDataModel> GetUser(Guid id)
    {
        await using var database = new AuthenticationDbContext();
        
        try
        {
            return await database.Users.FindAsync(id) ?? new UserDataModel();
        }
        catch (NullReferenceException ex)
        {
            throw ex;
        }
    }

    public async Task<UserDataModel> GetUser(string email)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            return await database.Users.Where(o => o.Email == email).FirstOrDefaultAsync() ?? new UserDataModel();
        }
        catch (NullReferenceException ex)
        {
            throw ex;
        }
    }

    public async Task<string> GetUserRole(string email)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            var user = await GetUser(email);
            var userRoles = await database.UserRoles.Where(u => u.UserId == user.Id).FirstOrDefaultAsync();
            var role = await database.Roles.Where(r => r.Id == userRoles!.RoleId).FirstOrDefaultAsync();

            if (role == null) await SeedRoles();

            return role!.Name;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<RefreshTokenDataModel?> GetToken(Guid UserId)
    {
        await using var database = new AuthenticationDbContext();

        try
        {
            return await database.Tokens.FirstOrDefaultAsync(u => u.UserId == UserId) ?? new RefreshTokenDataModel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
            Console.WriteLine(ex);
            return new RefreshTokenDataModel();
        }
    }

    public async Task<bool> AddToken(RefreshTokenDataModel token)
    {
        await using var database = new AuthenticationDbContext();

        var existingToken = await database.Tokens.FirstOrDefaultAsync(t => t.UserId == token.UserId);

        if (existingToken != null)
        {
            database.Tokens.Remove(existingToken);
        }

        await database.Tokens.AddAsync(token);
        await database.SaveChangesAsync();

        return true;
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

        var existingToken = await database.Tokens.FirstOrDefaultAsync(t => t.Token == token);

        if (existingToken != null)
        {
            database.Tokens.Remove(existingToken);
            await database.SaveChangesAsync();
            return true;
        }

        return false;

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
        catch
        {
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

            //TODO: change this shit
            if (roles != null) return false;

            await database.Roles.AddAsync(role);
            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateUser(UserDataModel model)
    {
        await using var database = new AuthenticationDbContext();

        var user = await database.Users.FindAsync(model.Id);

        if (user == null) return false;

        user.Email = model.Email;
        user.Roles = model.Roles;
        user.PasswordHash = model.PasswordHash;
        user.Salt = model.Salt;
        user.Status = model.Status;

        await database.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        await using var database = new AuthenticationDbContext();

        var user = await database.Users.FindAsync(id);

        if (user == null) return false;

        database.Users.Remove(user);
        await database.SaveChangesAsync();
        return true;

    }

    private async Task SeedRoles()
    {
        await using var database = new AuthenticationDbContext();

        await AddRole(Constants.RoleConstants.Moderator);
        await AddRole(Constants.RoleConstants.Default);
        await AddRole(Constants.RoleConstants.Admin);
    }

    //TODO: fill this method
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}