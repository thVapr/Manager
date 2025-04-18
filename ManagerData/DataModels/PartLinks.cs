using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("PartLinks")]
public class PartLinks
{
    public Guid MasterId { get; set; }
    public Guid SlaveId { get; set; }
}