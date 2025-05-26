using ManagerData.Management;

namespace ManagerLogic.Management;

public class PathHelper(ITaskRepository taskRepository) : IPathHelper
{
    public async Task CleanTaskPaths(Guid partId, int pathOrder)
    {
        var tasks = await taskRepository.GetEntitiesById(partId);
        foreach (var task in tasks!)
        {
            var nodes = ExtractNodesFromPath(task.Path!);
            nodes.Remove(pathOrder);
            task.Path = ConvertNodesToPath(nodes);
            await taskRepository.UpdateEntity(task);
        }
    }
    private List<int> ExtractNodesFromPath(string path)
    {
        return path.Split('-').Select(int.Parse).ToList();
    }

    private string ConvertNodesToPath(List<int> nodes)
    {
        return string.Join("-", nodes);
    }
}