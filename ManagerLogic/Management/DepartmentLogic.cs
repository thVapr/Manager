
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class DepartmentLogic : IManagementLogic<DepartmentModel>
{
    private readonly IManagementRepository<DepartmentDataModel> _repository;
    private readonly IManagementRepository<CompanyDataModel> _companyRepository;

    public DepartmentLogic(IManagementRepository<DepartmentDataModel> repository,
                           IManagementRepository<CompanyDataModel> companyRepository)
    {
        _repository = repository;
        _companyRepository = companyRepository;
    }

    public async Task<DepartmentModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        var department = new DepartmentModel
        {
            Name = entity.Name,
            Description = entity.Description,
        };
        
        return department;
    }

    public async Task<bool> CreateEntity(DepartmentModel model)
    {
        var entity = new DepartmentDataModel
        {
            Name = model.Name,
            Description = model.Description
        };

        return await _repository.CreateEntity(model.CompanyId, entity);
    }

    public Task<bool> UpdateEntity(DepartmentModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }
}