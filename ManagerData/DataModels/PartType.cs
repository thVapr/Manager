using System.ComponentModel.DataAnnotations;

namespace ManagerData.DataModels;

public class PartType
{
    [Key] public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    
    public IEnumerable<PartDataModel> Parts { get; set; } = new List<PartDataModel>();
}