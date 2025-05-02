
using ManagerData.DataModels.Authentication;
using ManagerLogic.Models;

namespace ManagerLogic.Authentication;

public interface IAuthentication
{
    Task<Tuple<string,string>> Authenticate(LoginModel user);
    Task<Tuple<string, string>> UpdateToken(RefreshModel model);

    Task<UserDataModel> GetUser(string email);
    Task<IEnumerable<string>> GetAdminIds();
    Task<ICollection<string>> GetAvailableUserIds();

    Task<bool> Logout(string token);

    Task<bool> CreateUser(LoginModel user);
    Task<bool> RemoveUser(Guid id);
}