using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class BackgroundTaskRepository(MainDbContext database) : IBackgroundTaskRepository
{
    public async Task<bool> Create(BackgroundTask task)
    {
        try
        {
            await database.BackgroundTasks.AddAsync(task);
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> Delete(Guid id)
    {
        try
        {
            var targetTask = await database.BackgroundTasks.FindAsync(id);

            if (targetTask == null)
                return false;

            database.BackgroundTasks.Remove(targetTask);
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<ICollection<BackgroundTask>> GetAllNearest()
    {
        try
        {
            var currentTime = DateTime.UtcNow;
            return await database.BackgroundTasks
                .Include(t => t.Part)
                .Include(t => t.Task)
                .Include(t => t.Member)
                .Where(task => (currentTime - task.Timeline) <= TimeSpan.FromMinutes(1))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }
}