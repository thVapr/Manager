
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class TaskRepository : IManagementRepository<TaskDataModel>
{
    public async Task<bool> CreateEntity(TaskDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingTask = await database.Tasks.Where(m => m.Name == model.Name).FirstOrDefaultAsync();

            if (existingTask != null) return false;

            await database.Tasks.AddAsync(model);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<TaskDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Tasks.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new TaskDataModel();
        }
        catch
        {
            return new TaskDataModel();
        }
    }

    public async Task<bool> UpdateEntity(TaskDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var task = await database.Tasks.FindAsync(model.Id);

            if (task == null) return false;

            task.Name = model.Name;
            task.Description = model.Description;
            task.EmployeeId = model.EmployeeId;
            task.Level = model.Level;

            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteEntity(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingTask = await database.Tasks.FindAsync(id);

            if (existingTask == null) return false;

            database.Tasks.Remove(existingTask);
            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}