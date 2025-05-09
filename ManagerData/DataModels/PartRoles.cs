namespace ManagerData.DataModels;

public class PartRole : BaseDataModel
{
    public Guid PartId { get; set; }
    public PartDataModel Part { get; set; } = null!;
}