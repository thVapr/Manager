using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("PartTypes")]
public class PartType
{
    [Key] public int Id { get; init; }
    public string Name { get; set; } = "";
    
    public IEnumerable<PartDataModel> Parts { get; set; }
}