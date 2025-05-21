using ManagerData.DataModels.Authentication;

namespace ManagerData.Authentication;

public interface IAuthenticationRepository
{
    Task<UserDataModel> GetUser(string email);
    Task<UserDataModel> GetUserById(Guid userId);
    Task<ICollection<UserDataModel>> GetUsers();
    Task<string> GetUserRole(string email);

    Task<RefreshTokenDataModel?> GetToken(Guid userId); 
    Task<RefreshTokenDataModel?> GetToken(string token); 
    Task<bool> AddToken(RefreshTokenDataModel token);
    Task<bool> UpdateToken(RefreshTokenDataModel tokenModel, string token);
    Task<bool> DeleteToken(string token);

    Task<IEnumerable<Guid>> GetAdminIds();

    Task<bool> AddRole(string name);
    
    Task<bool> AddUser(UserDataModel user);
    Task<bool> UpdateUser(UserDataModel user);
    Task<bool> DeleteUser(Guid id);
}