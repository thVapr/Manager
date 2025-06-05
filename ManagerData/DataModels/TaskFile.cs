namespace ManagerData.DataModels;

public class TaskFile
{
    public Guid TaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public TaskDataModel Task { get; set; }
}