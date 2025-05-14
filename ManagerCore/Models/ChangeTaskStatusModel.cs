using System.ComponentModel.DataAnnotations;

namespace ManagerCore.Models;

public class ChangeTaskStatusModel
{
    [Required] public string PartId { get; set; }
    [Required] public string TaskId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}