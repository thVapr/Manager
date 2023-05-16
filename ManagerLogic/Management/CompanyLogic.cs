
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

        if (entity.Id == Guid.Empty) return new CompanyModel();

        return new CompanyModel
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<IEnumerable<CompanyModel>> GetEntities()
    {
        var entities = await _repository.GetEntities();

        if (entities == null) return Enumerable.Empty<CompanyModel>();

        return entities
            .Where(e => e.Id != Guid.Empty)
            .Select(e => new CompanyModel
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                Description = e.Description
            })
            .ToList();
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
            Name = model.Name!,
            Description = model.Description!,
        };

        return await _repository.CreateEntity(entity);
    }

    public Task<bool> UpdateEntity(CompanyModel model)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CompanyModel>> GetEntitiesByQuery(string query, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }
}