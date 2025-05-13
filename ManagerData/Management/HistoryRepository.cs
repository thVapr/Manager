using System.Linq.Expressions;

using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class HistoryRepository : IHistoryRepository
{
    public async Task<bool> Create(TaskHistory model)
    {
        await using var database = new MainDbContext();

        try
        {
            var task = await database.Tasks
                .FirstOrDefaultAsync(x => x.Id == model.TaskId);
            if (task == null)
                return false;
            var sourceStatus = await database.PartTaskStatuses
                .FirstOrDefaultAsync(status => status.Order == model.SourceStatusId);
            if (sourceStatus == null)
                return false;
            var destinationStatus = await database.PartTaskStatuses
                .FirstOrDefaultAsync(status => status.Order == model.SourceStatusId);
            if (destinationStatus == null)
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
            Console.WriteLine(ex);
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
        await using var database = new MainDbContext();

        try
        {
            return await database.TaskHistories
                .Where(predicate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }

}