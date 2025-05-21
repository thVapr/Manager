namespace ManagerCore.ViewModels;

public class MemberViewModel
{
    public string? Id { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }

    public string? MessengerId { get; set; }
    public bool? IsMessengerConfirmed { get; set; }
    
    public string? PartId { get; set; }
    public string? PartName { get; set; }
}