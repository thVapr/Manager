
using System.ComponentModel.DataAnnotations;
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class MemberRepository : IMemberRepository
{
    public async Task<bool> CreateEntity(MemberDataModel model)
    {
        await using var database = new MainDbContext();

        try
        {
            await database.Members.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> CreateEntity(Guid id, MemberDataModel model)
    {
        model.Id = id;
        
        return await CreateEntity(model);
    }

    public Task<bool> AddToEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveFromEntity(Guid firstId, Guid secondId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LinkEntities(Guid masterId, Guid slaveId)
    {
        await using var database = new MainDbContext();

        try
        {
            var part = await database.Parts.Where(e => e.Id == masterId).FirstOrDefaultAsync();

            if (part == null) return false;

            await database.PartMembers.AddAsync(
                new PartMemberDataModel()
                {
                    PartId = masterId,
                    MemberId = slaveId,
                }
            );

            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public Task<bool> UnlinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public async Task<MemberDataModel> GetEntityById(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            return await database.Members.Where(m => m.Id == id)
                .FirstOrDefaultAsync() ?? new MemberDataModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new MemberDataModel();
        }
    }

    public Task<IEnumerable<MemberDataModel>?> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<MemberDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var memberIds = await database.PartMembers
                .Where(d => d.PartId == id)
                .Select(de => de.MemberId)
                .ToListAsync();

            var members = await database.Members
                .Where(e => memberIds.Contains(e.Id))
                .ToListAsync();

            return members;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new List<MemberDataModel>();
        }
    }

    public async Task<bool> UpdateEntity(MemberDataModel model)
    {
        await using var database = new MainDbContext();

        try
        {
            var employee = await database.Members.FindAsync(model.Id);

            if (employee == null) return false;

            if(!string.IsNullOrEmpty(model.FirstName))
                employee.FirstName = model.FirstName;
            if (!string.IsNullOrEmpty(model.LastName))
                employee.LastName = model.LastName;
            if (!string.IsNullOrEmpty(model.Patronymic))
                employee.Patronymic = model.Patronymic;

            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
    
    public async Task<bool> DeleteEntity(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var existingEmployee = await database.Members.FindAsync(id);

            if (existingEmployee == null) return false;

            database.Members.Remove(existingEmployee);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public void Dispose()
    {
    }

    public async Task<IEnumerable<MemberDataModel>> GetEmployeesWithoutProjectsByDepartmentId(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var links = await database.PartMembers.Select(pe => pe.MemberId).ToListAsync();
            var departmentLinks = await database.PartMembers
                .Where(d => d.PartId == id)
                .Select(de => de.MemberId)
                .ToListAsync();

            var employees = await database.Members
                .Where(e => !links.Contains(e.Id) && departmentLinks.Contains(e.Id))
                .ToListAsync();

            return employees;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new List<MemberDataModel>();
        }
    }

    public async Task<IEnumerable<MemberDataModel>> GetMembersWithoutPart()
    {
        await using var database = new MainDbContext();

        try
        {
            var employeeIds = await database.PartMembers.Select(de => de.MemberId).ToListAsync();

            return await database.Members.Where(e => !employeeIds.Contains(e.Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];

        }
    }

    public async Task<IEnumerable<MemberDataModel>> GetMembersFromPart(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var links = await database.PartMembers.Where(pe => pe.PartId == id).ToListAsync();
            var employeeIds = links.Select(l => l.MemberId);

            return await database.Members.Where(e => employeeIds.Contains(e.Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];

        }
    }

    public async Task<IEnumerable<MemberDataModel>> GetAvailableMembersFromPart(Guid id)
    {
        await using var database = new MainDbContext();

        try
        {
            var ids = await GetAvailableMemberIds(database, id);
            ids.UnionWith(await GetAvailableMemberIdsFromRoot(database, id));
            var currentPartMemberIds = await GetMembersFromPart(id);
            ids.ExceptWith(currentPartMemberIds.Select(member => member.Id));
            
            return await database.Members.Where(member => ids.Contains(member.Id)).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    private async Task<ISet<Guid>> GetAvailableMemberIdsFromRoot(MainDbContext database, Guid? rootPartId)
    {
        var memberIds = new HashSet<Guid>();
        var processedParts = new HashSet<Guid>();
        var partsToProcess = new Queue<Guid>();

        if (rootPartId.HasValue)
        {
            partsToProcess.Enqueue(rootPartId.Value);
        }

        while (partsToProcess.Count > 0)
        {
            var currentPartId = partsToProcess.Dequeue();

            var currentMembers = await database.PartMembers
                .Where(pm => pm.PartId == currentPartId)
                .Select(pm => pm.MemberId)
                .ToListAsync();

            memberIds.UnionWith(currentMembers);
            
            var childParts = await database.Parts
                .Where(p => p.MainPartId == currentPartId)
                .Select(p => p.Id)
                .ToListAsync();

            foreach (var childPartId in childParts)
            {
                if (!processedParts.Contains(childPartId))
                {
                    partsToProcess.Enqueue(childPartId);
                    processedParts.Add(childPartId);
                }
            }
        }

        return memberIds;
    }
    private async Task<ISet<Guid>> GetAvailableMemberIds(MainDbContext database, Guid? id)
    {
        var set = new HashSet<Guid>();
        Guid? mainPartId = id;
        
        while (mainPartId != null)
        {
            var currentPartId = mainPartId.Value;
            mainPartId = await database.Parts
                .Where(part => part.Id == currentPartId)
                .Select(data => data.MainPartId).FirstOrDefaultAsync();
            var links = await database.PartMembers
                .Where(pe => pe.PartId == mainPartId)
                .Select(pm => pm.MemberId).ToListAsync();
            set.UnionWith(links);
        }
        
        return set;
    }
}