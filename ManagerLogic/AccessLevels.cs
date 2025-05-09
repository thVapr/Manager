namespace ManagerLogic;

public enum AccessLevel : int
{
    Watch = 0,
    Read,
    Take,
    Create,
    Control,
    Leader
}