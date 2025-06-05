using ManagerData.Contexts;
using ManagerData.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagerData.Management.Implementation;

public class TaskTypeRepository(MainDbContext database, ILogger<TaskTypeRepository> logger) : ITaskTypeRepository
{
    public async Task<bool> Create(PartTaskType type)
    {
        try
        {
            database.PartTaskTypes.Add(type);
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
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
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
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
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
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
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }
}