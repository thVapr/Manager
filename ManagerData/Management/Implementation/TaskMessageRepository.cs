using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class TaskMessageRepository(MainDbContext context) : ITaskMessageRepository
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
        catch (Exception e)
        {
            Console.WriteLine(e);
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
        catch (Exception e)
        {
            Console.WriteLine(e);
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}