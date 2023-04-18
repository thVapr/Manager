
using ManagerData.Contexts;
using ManagerData.DataModels;
using ManagerData.DataModels.Authentication;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Authentication;

public class AuthenticationRepository : IAuthenticationRepository, IDisposable
{
    private readonly AuthenticationDbContext _context;

    public AuthenticationRepository(AuthenticationDbContext context)
    {
        _context = context;
        SeedRoles().Wait();
    }

    public async Task<UserDataModel> GetUser(Guid id)
    {
        try
        {
            return await _context.Users.FindAsync(id) ?? new UserDataModel();
        }
        catch (NullReferenceException ex)
        {
            throw ex;
        }
    }

    public async Task<UserDataModel> GetUser(string email)
    {
        try
        {
            return await _context.Users.Where(o => o.Email == email).FirstOrDefaultAsync() ?? new UserDataModel();
        }
        catch (NullReferenceException ex)
        {
            throw ex;
        }
    }

    public async Task<string> GetUserRole(string email)
    {
        try
        {
            var user = await GetUser(email);
            var userRoles = await _context.UserRoles.Where(u => u.UserId == user.Id).FirstOrDefaultAsync();
            var role = await _context.Roles.Where(r => r.Id == userRoles!.RoleId).FirstOrDefaultAsync();

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
        try
        {
            return await _context.Tokens.FirstOrDefaultAsync(u => u.UserId == UserId) ?? new RefreshTokenDataModel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<RefreshTokenDataModel?> GetToken(string token)
    {
        try
        {
            return await _context.Tokens.FirstOrDefaultAsync(u => u.Token == token) ?? new RefreshTokenDataModel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> AddToken(RefreshTokenDataModel token)
    {
        var existingToken = await _context.Tokens.FirstOrDefaultAsync(t => t.UserId == token.UserId);

        if (existingToken != null)
        {
            _context.Tokens.Remove(existingToken);
            await _context.SaveChangesAsync();
        }

        await _context.Tokens.AddAsync(token);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateToken(RefreshTokenDataModel tokenModel, string token)
    {
        var existingToken = _context.Tokens.FirstOrDefault(t => t.Token == token);

        if (existingToken is null) return false;

        await DeleteToken(token);
        await AddToken(tokenModel);
        
        return true;
    }

    public async Task<bool> DeleteToken(string token)
    {
        var existingToken = await _context.Tokens.FirstOrDefaultAsync(t => t.Token == token);

        if (existingToken != null)
        {
            _context.Tokens.Remove(existingToken);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;

    }

    public async Task<bool> AddUser(UserDataModel user)
    {
        try
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null) return false;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == Constants.RoleConstants.Default);

            var userRole = new UserRoleDataModel
            {
                UserId = user.Id,
                RoleId = role!.Id
            };

            _context.Users.Add(user);
            _context.UserRoles.Add(userRole);
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddRole(string name)
    {
        try
        {
            var role = new RoleDataModel
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            var roles = await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);

            //TODO: change this shit
            if (roles != null) return false;

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null) return false;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;

    }

    public async Task<bool> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;

    }

    public async Task SeedRoles()
    {
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