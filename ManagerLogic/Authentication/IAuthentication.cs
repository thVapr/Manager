
using ManagerLogic.Models;

namespace ManagerLogic.Authentication;

public interface IAuthentication
{
    Task<Tuple<string,string>> Authenticate(LoginModel user);
    Task<Tuple<string, string>> UpdateToken(RefreshModel model);
    Task<IEnumerable<string>> GetAdminIds();

    Task<bool> Logout(string token);

    Task<bool> CreateUser(LoginModel user);
}