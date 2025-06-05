using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class TaskMessageLogic(ITaskMessageRepository repository) : ITaskMessageLogic
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
        
        return await repository.CreateAsync(new TaskMessage
        {
            Id = Guid.NewGuid(),
            Message = message.Message,
            CreatorId = Guid.Parse(message.CreatorId!),
            TaskId = Guid.Parse(message.TaskId!)
        });
    }

    public async Task<bool> DeleteAsync(Guid messageId)
    {
        return await repository.DeleteAsync(messageId);
    }
}