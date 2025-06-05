using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagerData.Management.Implementation;

public class TaskMessageRepository(MainDbContext context, ILogger<TaskMessageRepository> logger) : ITaskMessageRepository
{
    public async Task<ICollection<TaskMessage>> GetTaskMessages(Guid taskId)
    {
        try
        {
            return await context.TaskMessages
                .Where(x => x.TaskId == taskId)
                .Include(x => x.Creator)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<bool> CreateAsync(TaskMessage message)
    {
        try
        {
            await context.TaskMessages.AddAsync(message);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid messageId)
    {
        try
        {
            var existMessage = await context.TaskMessages
                .FirstOrDefaultAsync(x => x.Id == messageId);
            if (existMessage == null)
                return false;
            context.TaskMessages.Remove(existMessage);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }
}