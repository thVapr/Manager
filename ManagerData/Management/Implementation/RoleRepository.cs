using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class RoleRepository(MainDbContext database) : IRoleRepository
{
    public async Task<bool> Create(PartRole role)
    {
        try
        {
            database.PartRoles.Add(role);
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> Change(PartRole role)
    {
        try
        {
            var existingRole = await database.PartRoles
                .FirstOrDefaultAsync(x => x.Id == role.Id && x.PartId == role.PartId);
            if (existingRole == null)
                return false;
            if(role.Name != existingRole.Name && role.Name != string.Empty)
                existingRole.Name = role.Name;
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> Delete(Guid partId, Guid roleId)
    {
        try
        {
            var existingRole = await database.PartRoles
                .FirstOrDefaultAsync(x => x.Id == roleId && x.PartId == partId);
            if (existingRole == null) 
                return false;

            database.PartRoles.Remove(existingRole);
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> SetRole(Guid partId, Guid roleId, Guid memberId)
    {
        try
        {
            var member = await database.Members
                .FirstOrDefaultAsync(x => x.Id == memberId);
            if (member == null)
                return false;
            var association = await database.PartRoles
                .FirstOrDefaultAsync(x => x.Id == roleId && x.PartId == partId);
            if (association == null)
                return false;
            var part = await database.Parts.FirstOrDefaultAsync(x => x.Id == partId);
            if (part == null)
                return false;
            await database.PartMemberRoles.AddAsync(new PartMemberRole
            {
                PartId = partId,
                PartRoleId = roleId,
                MemberId = memberId
            });
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> RemoveRole(Guid partId,Guid roleId, Guid memberId)
    {
        try
        {
            var existingAssociation = await database.PartMemberRoles
                .FirstOrDefaultAsync(x => x.PartRoleId == roleId && x.MemberId == memberId && x.PartId == partId);
            if (existingAssociation == null)
                return false;

            database.PartMemberRoles.Remove(existingAssociation);
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<ICollection<PartRole>> GetByMemberId(Guid partId, Guid memberId)
    {
        try
        {
            var associations = await database.PartMemberRoles
                .Where(x => x.MemberId == memberId && x.PartId == partId)
                .Select(a => a.PartRoleId).ToListAsync();
            return await database.PartRoles.Where(x => associations.Contains(x.Id)).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public async Task<ICollection<PartRole>> GetByPartId(Guid partId)
    {
        try
        {
            return await database.PartRoles.Where(x => x.PartId == partId).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public void Dispose()
    {
        
    }
}