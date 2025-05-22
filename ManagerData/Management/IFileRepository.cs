namespace ManagerData.Management;

public interface IFileRepository
{
    Task<List<string>> ListFilesAsync(Guid taskId);
    Task UploadFileAsync(Guid taskId, string objectName, Stream data, string contentType);
    Task<Stream> GetFileAsync(string objectName, Guid taskId);
}