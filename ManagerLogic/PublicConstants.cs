
namespace ManagerLogic;

public abstract class PublicConstants
{
    public const string Id = "id";
    public const string Email = "email";
    public const string Role = "role";

    public enum Task
    {
        Todo,
        Doing,
        Done
    }
}