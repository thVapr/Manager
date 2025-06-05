using ManagerLogic.Models;
using Microsoft.AspNetCore.Http;

namespace ManagerLogic.Management;

public interface IFileLogic
{
    Task<List<TaskFileModel>> GetFileList(string taskId);
    Task Upload(IFormFile file, string taskId);
    Task<Stream> Download(string filename, string taskId);
    Task Remove(string filename, string taskId);
}