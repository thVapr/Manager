
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
            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CreateEntity(Guid id, EmployeeDataModel model)
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
            var department = await database.Departments.Where(e => e.Id == firstId).FirstOrDefaultAsync();

            if (department == null) return false;

            await database.DepartmentEmployees.AddAsync(
                new DepartmentEmployeesDataModel()
                {
                    DepartmentId = firstId,
                    EmployeeId = secondId,
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

    public async Task<EmployeeDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Employees.Where(m => m.UserId == id).FirstOrDefaultAsync() ?? new EmployeeDataModel();
        }
        catch
        {
            return new EmployeeDataModel();
        }
    }

    public Task<IEnumerable<EmployeeDataModel>?> GetEntities()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<EmployeeDataModel>?> GetEntitiesById(Guid id)
    {
        throw new NotImplementedException();
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

    public void Dispose()
    {
    }
}