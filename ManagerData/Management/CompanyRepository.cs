
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class CompanyRepository : IManagementRepository<CompanyDataModel>
{
    public async Task<bool> CreateEntity(CompanyDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingCompany = await database.Companies.Where(m => m.Name == model.Name).FirstOrDefaultAsync();

            if (existingCompany != null) return false;

            await database.Companies.AddAsync(model);
            await database.SaveChangesAsync();

            return true;
        }
        catch(Exception ex) 
        {
            return false;
        }
    }

    public Task<bool> CreateEntity(Guid id, CompanyDataModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyDataModel> GetEntityById(Guid id)
    {
        await using var database = new ManagerDbContext();

        try
        {
            return await database.Companies.Where(m => m.Id == id).FirstOrDefaultAsync() ?? new CompanyDataModel();
        }
        catch 
        {
            return new CompanyDataModel();
        }
    }

    public async Task<bool> UpdateEntity(CompanyDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var company = await database.Companies.FindAsync(model.Id);

            if (company == null) return false;
            
            company.Name = model.Name;
            company.Description = model.Description;

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