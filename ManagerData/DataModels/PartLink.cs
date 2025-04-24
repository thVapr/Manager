using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("PartLinks")]
public class PartLink
{
    public Guid MasterId { get; set; }
    public Guid SlaveId { get; set; }
    
    public PartDataModel MasterPart { get; set; } = null!;
    public PartDataModel SlavePart { get; set; } = null!;
}