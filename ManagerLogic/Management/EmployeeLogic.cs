
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class EmployeeLogic : IManagementLogic<EmployeeModel>
{
    private readonly IManagementRepository<EmployeeDataModel> _repository;

    public EmployeeLogic(IManagementRepository<EmployeeDataModel> repository)
    {
        _repository = repository;
    }
    
    public async Task<EmployeeModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        var department = new EmployeeModel
        {
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Patronymic = entity.Patronymic,
        };
        
        return department;
    }

    public async Task<bool> CreateEntity(EmployeeModel model)
    {
        var entity = new EmployeeDataModel
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Patronymic = model.Patronymic,
        };

        return await _repository.CreateEntity(model.DepartmentId, entity);
    }

    public Task<bool> UpdateEntity(EmployeeModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }
}