using ManagerLogic.Models;
using ManagerData.DataModels.Authentication;

namespace ManagerLogic.Authentication;

public interface IAuthentication
{
    Task<Tuple<string,string>> Authenticate(LoginModel user);
    Task<Tuple<string, string>> UpdateToken(RefreshModel model);

    Task<UserDataModel> GetUser(string email);
    Task<UserDataModel> GetUserById(Guid id);
    Task<UserDataModel> GetUserByMessengerId(string id);
    Task<IEnumerable<string>> GetAdminIds();
    Task<ICollection<string>> GetAvailableUserIds();

    Task<bool> Logout(string token);

    Task<bool> CreateUser(LoginModel user);
    Task<bool> UpdateUser(UserDataModel user);
    Task<bool> RemoveUser(Guid id);
}