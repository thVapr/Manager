using ManagerLogic.Models;

namespace ManagerCore.Models;

public class UpdateTaskRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TaskModel Task { get; set; }
}