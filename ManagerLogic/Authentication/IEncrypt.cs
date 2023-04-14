namespace ManagerLogic.Authentication;

public interface IEncrypt
{
    string HashPassword(string password, string salt);
}