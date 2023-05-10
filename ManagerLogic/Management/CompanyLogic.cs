
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class CompanyLogic : IManagementLogic<CompanyModel>
{
    private readonly IManagementRepository<CompanyDataModel> _repository;

    public CompanyLogic(IManagementRepository<CompanyDataModel> repository)
    {
        _repository = repository;
    }

    public async Task<CompanyModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        var company = new CompanyModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };

        return company;
    }

    public async Task<IEnumerable<CompanyModel>> GetEntities()
    {
        var entities = await _repository.GetEntities();
        List<CompanyModel> result = new();

        if (entities == null) return result;

        result.AddRange(
            entities.Select(
                v => new CompanyModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Description = v.Description
                }));

        return result;
    }

    public Task<IEnumerable<CompanyModel>> GetEntitiesById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CreateEntity(CompanyModel model)
    {
        var entity = new CompanyDataModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description
        };

        return await _repository.CreateEntity(entity);
    }

    public Task<bool> UpdateEntity(CompanyModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }
}