namespace ManagerData.DataModels;

public class TaskFile
{
    public Guid TaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    
    public TaskDataModel Task { get; set; }
}