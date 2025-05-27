using System.ComponentModel.DataAnnotations;

namespace ManagerData.DataModels;

public class PartTaskType
{
    [Key] public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public Guid PartId { get; set; }
    
    public PartDataModel Part { get; set; }
    public ICollection<TaskDataModel> Tasks { get; set; }
    
}