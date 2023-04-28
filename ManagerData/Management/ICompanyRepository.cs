using ManagerData.DataModels;

namespace ManagerData.Management;

public interface ICompanyRepository
{
    public Task<bool> CreateCompany(CompanyDataModel model);
    public Task<CompanyDataModel> GetCompany(Guid id);
    public Task<bool> UpdateCompany(CompanyDataModel model);
    public Task<bool> DeleteCompany(Guid id);
}