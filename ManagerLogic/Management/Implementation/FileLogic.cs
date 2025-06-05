using ManagerData.Management;
using Microsoft.AspNetCore.Http;

namespace ManagerLogic.Management;

public class FileLogic(IFileRepository fileRepository) : IFileLogic
{
    public async Task<List<string>> GetFileList(string taskId)
    {
        return await fileRepository.ListFilesAsync(Guid.Parse(taskId));
    }

    public async Task Upload(IFormFile file, string taskId)
    {
        await using var stream = file.OpenReadStream();
        await fileRepository.UploadFileAsync(Guid.Parse(taskId), file.FileName, stream, file.ContentType);
    }

    public async Task<Stream> Download(string filename, string taskId)
    {
        var fileStream = await fileRepository.GetFileAsync(filename, Guid.Parse(taskId));
        return fileStream;
    }
}