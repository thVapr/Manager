
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class EmployeeRepository : IManagementRepository<EmployeeDataModel>
{
    public async Task<bool> CreateEntity(EmployeeDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingEmployee = await database.Employees.Where(m => m.UserId == model.UserId).FirstOrDefaultAsync();

            if (existingEmployee != null) return false;

            await database.Employees.AddAsync(model);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<EmployeeDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Employees.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new EmployeeDataModel();
        }
        catch
        {
            return new EmployeeDataModel();
        }
    }

    public async Task<bool> UpdateEntity(EmployeeDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var employee = await database.Employees.FindAsync(model.Id);

            if (employee == null) return false;

            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.Patronymic = model.Patronymic;

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
            var existingEmployee = await database.Employees.FindAsync(id);

            if (existingEmployee == null) return false;

            database.Employees.Remove(existingEmployee);
            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}