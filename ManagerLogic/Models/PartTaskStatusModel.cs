namespace ManagerLogic.Models;

public class PartTaskStatusModel : BaseModel
{
    public int? GlobalStatus { get; set; }
    public int? Order { get; set; }
    public bool? IsFixed { get; set; }
    
    public int? AccessLevel { get; set; }
    public string PartId { get; set; }
    public string? PartRoleId { get; set; }
}