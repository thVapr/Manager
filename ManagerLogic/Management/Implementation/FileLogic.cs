using ManagerData.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Http;

namespace ManagerLogic.Management;

public class FileLogic(IFileRepository fileRepository) : IFileLogic
{
    public async Task<List<TaskFileModel>> GetFileList(string taskId)
    {
        return (await fileRepository.ListFilesAsync(Guid.Parse(taskId)))
            .Select(file => new TaskFileModel
            {
                FileName = file.FileName,
                CreatedAt = file.CreatedAt
            }).ToList();
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

    public async Task Remove(string filename, string taskId)
    {
        await fileRepository.RemoveFileAsync(filename, Guid.Parse(taskId));
    }
}