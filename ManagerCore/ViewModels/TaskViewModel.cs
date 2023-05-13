namespace ManagerCore.ViewModels;

public class TaskViewModel
{
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }

    public string CreatorId { get; set; }
    public string CreatorName { get; set; }

    public int Level { get; set; }
    public int Status { get; set; }
}