
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Management;

public class CompanyRepository : ICompanyRepository
{
    public async Task<bool> CreateCompany(CompanyDataModel model)
    {
        await using var database = new ManagerDbContext();

        try
        {
            var existingCompany = await database.Companies.Where(m => m.Name == model.Name).FirstOrDefaultAsync();

            if (existingCompany != null) return false;

            await database.Companies.AddAsync(model);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CompanyDataModel> GetCompany(Guid id)
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

    public async Task<bool> UpdateCompany(CompanyDataModel model)
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

    public async Task<bool> DeleteCompany(Guid id)
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
}