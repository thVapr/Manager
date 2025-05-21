
using System.ComponentModel.DataAnnotations;

namespace ManagerData.DataModels;

public abstract class BaseDataModel
{
    [Key] public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}