using System.Linq.Expressions;
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagerData.Management.Implementation;

public class HistoryRepository(MainDbContext database, ILogger<HistoryRepository> logger) : IHistoryRepository
{
    public async Task<bool> Create(TaskHistory model)
    {
        try
        {
            var task = await database.Tasks
                .FirstOrDefaultAsync(x => x.Id == model.TaskId);
            if (task == null)
                return false;
            var initiator = await database.Members
                .FirstOrDefaultAsync(member => member.Id == model.InitiatorId);
            if (initiator == null)
                return false;
            
            await database.TaskHistories.AddAsync(model);
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<ICollection<TaskHistory>> GetByTaskId(Guid taskId)
    {
        return await GetByPredicate(x => x.TaskId == taskId);
    }

    public async Task<ICollection<TaskHistory>> GetByMemberId(Guid memberId)
    {
        return await GetByPredicate(x => x.InitiatorId == memberId);
    }
    
    private async Task<ICollection<TaskHistory>> GetByPredicate(Expression<Func<TaskHistory, bool>> predicate)
    {
        try
        {
            return await database.TaskHistories
                .Where(predicate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

}