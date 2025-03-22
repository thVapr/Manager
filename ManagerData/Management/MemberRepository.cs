
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class MemberRepository : IMemberRepository
{
    public async Task<bool> CreateEntity(MemberDataModel model)
    {
        await using var database = new ManagerDbContext();

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
        await CreateEntity(model);
        await using var database = new ManagerDbContext();

        try
        {
            await LinkEntities(id, model.Id);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> LinkEntities(Guid firstId, Guid secondId)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var department = await database.Parts.Where(e => e.Id == firstId).FirstOrDefaultAsync();

            if (department == null) return false;

            await database.PartMembers.AddAsync(
                new PartMembersDataModel()
                {
                    PartId = firstId,
                    MemberId = secondId,
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

    public Task<bool> UnlinkEntities(Guid firstId, Guid secondId)
    {
        throw new NotImplementedException();
    }

    public async Task<MemberDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Members.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new MemberDataModel();
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
        await using var database = new ManagerDbContext();

        try
        {
            var employeeIds = await database.PartMembers
                .Where(d => d.PartId == id)
                .Select(de => de.MemberId)
                .ToListAsync();

            var employees = await database.Members
                .Where(e => employeeIds.Contains(e.Id))
                .ToListAsync();

            return employees;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new List<MemberDataModel>();
        }
    }

    public async Task<bool> UpdateEntity(MemberDataModel model)
    {
        await using var database = new ManagerDbContext();

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
        await using var database = new ManagerDbContext();

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
        await using var database = new ManagerDbContext();

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

    public async Task<IEnumerable<MemberDataModel>> GetMembersWithoutPart(int level)
    {
        await using var database = new ManagerDbContext();

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
        await using var database = new ManagerDbContext();

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
}