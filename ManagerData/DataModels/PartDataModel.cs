using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Parts")]
public class PartDataModel: BaseDataModel
{
    public int Level { get; set; } = 0;
    public Guid? TypeId { get; set; }
    
    public IEnumerable<PartLink>? SubParts { get; set; }
    public PartLink? MainPart { get; set; }
    public IEnumerable<PartTasksDataModel>? PartTasks { get; set; }
    public IEnumerable<PartMembersDataModel>? PartMembers { get; set; }
}