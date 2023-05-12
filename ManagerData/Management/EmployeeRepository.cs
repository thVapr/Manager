
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class EmployeeRepository : IEmployeeRepository
{
    public async Task<bool> CreateEntity(EmployeeDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            await database.Employees.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
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

    public Task<bool> UnlinkEntities(Guid firstId, Guid secondId)
    {
        throw new NotImplementedException();
    }

    public async Task<EmployeeDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Employees.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new EmployeeDataModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new EmployeeDataModel();
        }
    }

    public Task<IEnumerable<EmployeeDataModel>?> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<EmployeeDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var employeeIds = await database.DepartmentEmployees
                .Where(d => d.DepartmentId == id)
                .Select(de => de.EmployeeId)
                .ToListAsync();

            var employees = await database.Employees
                .Where(e => employeeIds.Contains(e.Id))
                .ToListAsync();

            return employees;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new List<EmployeeDataModel>();
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
        catch (Exception ex)
        {
            Console.WriteLine(ex);
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
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public void Dispose()
    {
    }

    public async Task<IEnumerable<EmployeeDataModel>> GetEmployeesWithoutProjectsByDepartmentId(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var links = await database.ProjectEmployees.Select(pe => pe.EmployeeId).ToListAsync();
            var departmentLinks = await database.DepartmentEmployees
                .Where(d => d.DepartmentId == id)
                .Select(de => de.EmployeeId)
                .ToListAsync();

            var employees = await database.Employees
                .Where(e => !links.Contains(e.Id) && departmentLinks.Contains(e.Id))
                .ToListAsync();

            return employees;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new List<EmployeeDataModel>();
        }
    }

    public async Task<IEnumerable<EmployeeDataModel>> GetEmployeesWithoutDepartments()
    {
        await using var database = new ManagerDbContext();

        try
        {
            var employeeIds = await database.DepartmentEmployees.Select(de => de.EmployeeId).ToListAsync();

            return await database.Employees.Where(e => !employeeIds.Contains(e.Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new List<EmployeeDataModel>();

        }
    }

    public async Task<IEnumerable<EmployeeDataModel>> GetEmployeesFromProject(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var links = await database.ProjectEmployees.ToListAsync();
            var employeeIds = links.Select(l => l.EmployeeId);

            return await database.Employees.Where(e => employeeIds.Contains(e.Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new List<EmployeeDataModel>();

        }
    }
}