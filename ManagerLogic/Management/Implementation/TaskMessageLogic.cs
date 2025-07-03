using ManagerData.Constants;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Background;
using ManagerLogic.Models;

namespace ManagerLogic.Management.Implementation;

public class TaskMessageLogic(
    ITaskMessageRepository repository,
    ITaskLogic taskLogic,
    IBackgroundTaskRepository backgroundTaskRepository,
    IHistoryRepository historyRepository) : ITaskMessageLogic
{
    public async Task<ICollection<TaskMessageModel>> GetTaskMessages(Guid taskId)
    {
        return (await repository.GetTaskMessages(taskId)).Select(message => new TaskMessageModel
        {
            Id = message.Id.ToString(),
            Message = message.Message,
            Creator = new MemberModel
            {
                Id = message.CreatorId.ToString(),
                FirstName = message.Creator.FirstName,
                LastName = message.Creator.LastName,
                Patronymic = message.Creator.Patronymic,
            },
            CreatedAt = message.CreatedAt
        }).ToList();
    }

    public async Task<bool> CreateAsync(TaskMessageModel message)
    {
        if (new []{ message.CreatorId, message.TaskId }.Any(string.IsNullOrEmpty))
            return false;
        
        var isMessageCreated = await repository.CreateAsync(new TaskMessage
        {
            Id = Guid.NewGuid(),
            Message = message.Message,
            CreatorId = Guid.Parse(message.CreatorId!),
            TaskId = Guid.Parse(message.TaskId!)
        });

        if (isMessageCreated)
        {
            var historyId = Guid.NewGuid();
            await historyRepository.Create(new TaskHistory
            {
                Id = historyId,
                TaskId = Guid.Parse(message.TaskId!),
                InitiatorId = Guid.Parse(message.CreatorId!),
                ActionType = TaskActionType.Commented,
            });
            var taskMembers = await taskLogic.GetTaskMembers(Guid.Parse(message.TaskId!));
            foreach (var member in taskMembers)
            {
                if (member.Id == message.CreatorId)
                    continue;
                await backgroundTaskRepository.Create(new BackgroundTask
                {
                    TaskId = Guid.Parse(message.TaskId!),
                    MemberId = Guid.Parse(member.Id!),
                    Message = message.Message,
                    HistoryId = historyId,
                    Timeline = DateTime.UtcNow,
                    Type = (int)BackgroundTaskType.Commented,
                });
            }
        }
        return isMessageCreated;
    }

    public async Task<bool> DeleteAsync(Guid messageId)
    {
        return await repository.DeleteAsync(messageId);
    }
}