using ManagerData.Constants;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Background;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Http;

namespace ManagerLogic.Management;

public class FileLogic(
    IFileRepository fileRepository, 
    IHistoryRepository historyRepository,
    IBackgroundTaskRepository backgroundTaskRepository,
    ITaskLogic taskLogic) : IFileLogic
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

    public async Task Upload(HistoryModel historyModel, IFormFile file, string taskId)
    {
        await using var stream = file.OpenReadStream();
        await fileRepository.UploadFileAsync(Guid.Parse(taskId), file.FileName, stream, file.ContentType);
        var historyId = Guid.NewGuid();
        await historyRepository.Create(new TaskHistory
        {
            Id = historyId,
            Name = file.FileName,
            TaskId = Guid.Parse(taskId),
            InitiatorId = Guid.Parse(historyModel.InitiatorId!),
            ActionType = TaskActionType.FileAdded
        });
        var taskMembers = await taskLogic.GetTaskMembers(Guid.Parse(taskId));
        foreach (var member in taskMembers)
        {
            if (member.Id == historyModel.InitiatorId)
                continue;
            await backgroundTaskRepository.Create(new BackgroundTask
            {
                HistoryId = historyId,
                TaskId = Guid.Parse(taskId),
                PartId = Guid.Parse(historyModel.PartId!),
                MemberId = Guid.Parse(member.Id!),
                Message = file.FileName,
                Timeline = DateTime.UtcNow,
                Type = (int)BackgroundTaskType.FileAdded,
            });
        }
    }

    public async Task<Stream> Download(string filename, string taskId)
    {
        var fileStream = await fileRepository.GetFileAsync(filename, Guid.Parse(taskId));
        return fileStream;
    }

    public async Task Remove(HistoryModel historyModel, string filename, string taskId)
    {
        await fileRepository.RemoveFileAsync(filename, Guid.Parse(taskId));
        var historyId = Guid.NewGuid();
        await historyRepository.Create(new TaskHistory
        {
            Id = historyId,
            Name = filename,
            TaskId = Guid.Parse(taskId),
            InitiatorId = Guid.Parse(historyModel.InitiatorId!),
            ActionType = TaskActionType.FileRemoved
        });
        var taskMembers = await taskLogic.GetTaskMembers(Guid.Parse(taskId));
        foreach (var member in taskMembers)
        {
            if (member.Id == historyModel.InitiatorId)
                continue;
            await backgroundTaskRepository.Create(new BackgroundTask
            {
                HistoryId = historyId,
                TaskId = Guid.Parse(taskId),
                PartId = Guid.Parse(historyModel.PartId!),
                MemberId = Guid.Parse(member.Id!),
                Message = filename,
                Timeline = DateTime.UtcNow,
                Type = (int)BackgroundTaskType.FileRemoved,
            });
        }
    }
}