using System.ComponentModel.DataAnnotations;

namespace ManagerCore.Models;

public class ChangeTaskStatusModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Forward { get; set; } = true;
}