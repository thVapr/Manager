namespace ManagerCore.ViewModels;

public class EmployeeViewModel
{
    public string? Id { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }

    public string? CompanyId { get; set; }
    public string? CompanyName { get; set; }

    public string? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }

    public string? ProjectId { get; set; }
    public string? ProjectName { get; set; }
}