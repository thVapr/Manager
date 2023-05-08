

using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class DepartmentRepository : IManagementRepository<DepartmentDataModel>
{
    public async Task<bool> CreateEntity(DepartmentDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingDepartment = await database.Departments.Where(m => m.Name == model.Name).FirstOrDefaultAsync();
            
            if (existingDepartment != null) return false;

            await database.Departments.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CreateEntity(Guid companyId, DepartmentDataModel model)
    {
        await CreateEntity(model);
        await using var database = new ManagerDbContext();

        try
        {
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

    public Task<bool> LinkEntities(Guid firstId, Guid secondId)
    {
        throw new NotImplementedException();
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

    public async Task<bool> UpdateEntity(DepartmentDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var department = await database.Companies.FindAsync(model.Id);

            if (department == null) return false;

            department.Name = model.Name;
            department.Description = model.Description;

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
            var existingCompany = await database.Companies.FindAsync(id);

            if (existingCompany == null) return false;

            database.Companies.Remove(existingCompany);
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