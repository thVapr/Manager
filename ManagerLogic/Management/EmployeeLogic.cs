
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class EmployeeLogic : IEmployeeLogic
{
    private readonly IManagementRepository<EmployeeDataModel> _repository;

    public EmployeeLogic(IManagementRepository<EmployeeDataModel> repository)
    {
        _repository = repository;
    }
    
    public async Task<EmployeeModel> GetEntityById(Guid id)
    {
        var entity = await _repository.GetEntityById(id);

        var employee = new EmployeeModel
        {
            UserId = entity.UserId.ToString(),
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Patronymic = entity.Patronymic,
        };
        
        return employee;
    }

    public Task<IEnumerable<EmployeeModel>> GetEntities()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<EmployeeModel>> GetEntitiesById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CreateEntity(EmployeeModel model)
    {
        var entity = new EmployeeDataModel
        {
            UserId = Guid.Parse(model.UserId),
            FirstName = model.FirstName,
            LastName = model.LastName,
            Patronymic = model.Patronymic,
        };

        return await _repository.CreateEntity(entity);
    }

    public Task<bool> UpdateEntity(EmployeeModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEntity(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddEmployeeToDepartment(Guid departmentId, Guid employeeId)
    {
        try
        {
            await _repository.LinkEntities(departmentId, employeeId);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}