namespace ManagerData.DataModels;

public class PartTaskStatus : BaseDataModel
{
    public int GlobalStatus { get; set; }
    public int Order { get; set; }
    public bool IsFixed { get; set; }
    
    public int AccessLevel { get; set; }
    public Guid PartId { get; set; }
    
    public PartDataModel Part { get; set; }
}