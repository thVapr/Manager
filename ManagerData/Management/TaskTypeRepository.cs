using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class TaskTypeRepository(MainDbContext database) : ITaskTypeRepository
{
    public async Task<bool> Create(PartTaskType type)
    {
        try
        {
            database.PartTaskTypes.Add(type);
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }    
    }

    public async Task<bool> Change(PartTaskType type)
    {
        try
        {
            var existingType = await database.PartTaskTypes
                .FirstOrDefaultAsync(x => x.Id == type.Id && x.PartId == type.PartId);
            if (existingType == null)
                return false;
            if(type.Name != existingType.Name && type.Name != string.Empty)
                existingType.Name = type.Name;
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> Delete(Guid partId, Guid typeId)
    {
        try
        {
            var existingType = await database.PartTaskTypes
                .FirstOrDefaultAsync(x => x.Id == typeId && x.PartId == partId);
            if (existingType == null) 
                return false;

            database.PartTaskTypes.Remove(existingType);
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<ICollection<PartTaskType>> GetByPartId(Guid partId)
    {
        try
        {
            return await database.PartTaskTypes
                .Where(x => x.PartId == partId).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }
}