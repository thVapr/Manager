namespace ManagerLogic.Management;

public interface IPathHelper
{
    Task CleanTaskPaths(Guid partId, int pathOrder);
}