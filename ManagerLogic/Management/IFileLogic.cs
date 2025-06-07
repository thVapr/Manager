using ManagerLogic.Models;
using Microsoft.AspNetCore.Http;

namespace ManagerLogic.Management;

public interface IFileLogic
{
    Task<List<TaskFileModel>> GetFileList(string taskId);
    Task Upload(HistoryModel historyModel, IFormFile file, string taskId);
    Task<Stream> Download(string filename, string taskId);
    Task Remove(HistoryModel historyModel, string filename, string taskId);
}