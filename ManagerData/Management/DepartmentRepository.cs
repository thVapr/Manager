

using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class DepartmentRepository : IManagementRepository<DepartmentDataModel>
{
    public async Task<bool> CreateEntity(DepartmentDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            await database.Departments.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> CreateEntity(Guid companyId, DepartmentDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingDepartment = await database.Departments.Where(m => m.Name == model.Name).FirstOrDefaultAsync();
            var existingLink = await database.CompanyDepartments.Where(m => m.CompanyId == companyId).FirstOrDefaultAsync();

            if (existingDepartment != null && existingLink != null) return false;

            await CreateEntity(model);

            var company = await database.Companies.Where(e => e.Id == companyId).FirstOrDefaultAsync();

            if (company == null) return false;

            await database.CompanyDepartments.AddAsync(
                new CompanyDepartmentsDataModel
                {
                    CompanyId = companyId, DepartmentId = model.Id
                });
            await database.SaveChangesAsync();

            return true;
        }
        catch(Exception ex) 
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

    public async Task<bool> UnlinkEntities(Guid firstId, Guid secondId)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var link = await database.DepartmentEmployees
                .Where(de => de.DepartmentId == firstId && de.EmployeeId == secondId)
                .FirstOrDefaultAsync();
            var secondLink = await database.ProjectEmployees
                .Where(pe => pe.EmployeeId == secondId)
                .FirstOrDefaultAsync();

            if (link == null) return false;

            database.DepartmentEmployees.Remove(link);
            
            if (secondLink != null)
                database.ProjectEmployees.Remove(secondLink);

            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<DepartmentDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Departments.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new DepartmentDataModel();
        }
        catch
        {
            return new DepartmentDataModel();
        }
    }

    public Task<IEnumerable<DepartmentDataModel>?> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<DepartmentDataModel>?> GetEntitiesById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var departments = await database.CompanyDepartments.Where(c => c.CompanyId == id).ToListAsync();
            List<DepartmentDataModel?> result = new();

            if (result == null) throw new ArgumentNullException(nameof(result));

            foreach (var v in departments)
            {
                result.Add(await database.Departments.Where(d => d.Id == v.DepartmentId).FirstOrDefaultAsync());
            }

            return result!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Enumerable.Empty<DepartmentDataModel>();
        }
    }

    public async Task<bool> UpdateEntity(DepartmentDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var department = await database.Departments.Where(c => c.Id == model.Id).FirstOrDefaultAsync();

            if (department == null) return false;

            if (!string.IsNullOrEmpty(model.Name))
                department.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Description))
                department.Description = model.Description;
            if (model.ManagerId != null)
                department.ManagerId = model.ManagerId;

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
            var existingCompany = await database.Companies.FindAsync(id);

            if (existingCompany == null) return false;

            database.Companies.Remove(existingCompany);
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
}