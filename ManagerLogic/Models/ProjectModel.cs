
namespace ManagerLogic.Models;

public class ProjectModel
{
    public int Name { get; set; }
    public string Description { get; set; }

    public IEnumerable<TaskModel> Tasks { get; set; }
}