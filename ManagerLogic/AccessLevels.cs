namespace ManagerCore.Utils;

public enum AccessLevel : int
{
    Watch,
    Read,
    Take,
    // 3-6 reserved for specific groups
    Create = 7,
    Update,
    Control,
    Leader
}