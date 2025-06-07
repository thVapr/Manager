using ManagerLogic.Models;
using ManagerData.Constants;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Background;
using Microsoft.AspNetCore.SignalR;

namespace ManagerLogic.Management;

public class TaskLogic(
    ITaskRepository repository,
    IPartLogic partLogic,
    IMemberLogic memberLogic,
    IHistoryRepository historyRepository,
    IBackgroundTaskRepository backgroundTaskRepository,
    IHubContext<UpdateHub> hubContext
    ) : ITaskLogic
{
    public async Task<TaskModel> GetEntityById(Guid id)
    {
        var task = await repository.GetEntityById(id);
        return ConvertToLogicModel(task!);
    }

    public async Task<ICollection<TaskModel>> GetEntities()
    {
        var entities = await repository.GetEntities();

        if (entities == null) 
            return [];

        return entities.Select(task => ConvertToLogicModel(task!)).ToList();
    }

    public async Task<ICollection<TaskModel>> GetEntitiesById(Guid id)
    {
        var tasks = await repository.GetEntitiesById(id);

        if (tasks == null) return [];

        return tasks.Select(task => ConvertToLogicModel(task!)).ToList();
    }

    public async Task<bool> CreateEntity(TaskModel model)
    {
        var entity = new TaskDataModel
        {
            Id = Guid.NewGuid(),

            Name = model.Name!,
            Description = model.Description ?? "",

            CreatorId = model.CreatorId,
            PartId = model.PartId,
            
            StartTime = model.StartTime,
            Deadline = model.Deadline,
            ClosedAt = model.ClosedAt,
            Path = model.Path!,
            TaskTypeId = model.TaskTypeId,
            
            Status = model.Status,
            Level = model.Level,
            Priority = model.Priority,
        };
        if (string.IsNullOrEmpty(model.Path))
        {
            var statuses = (await partLogic.GetPartTaskStatuses(model.PartId ?? Guid.Empty))
                .OrderBy(status => status.Order)
                .Select(status => status.Order).SkipLast(1);
            entity.Path = string.Join("-", statuses);
        }
        
        var isTaskCreated = await repository.CreateEntity((Guid)model.PartId!, entity);
        if (isTaskCreated)
        {
            await historyRepository.Create(new TaskHistory
            {
                TaskId = entity.Id,
                SourceStatusId = entity.Status,
                DestinationStatusId = null,
                InitiatorId = entity.CreatorId,
                Description = "",
                Name = "",
                ActionType = TaskActionType.Created,
            });
            var members = await GetAvailableMembersForTask(
                entity.PartId ?? Guid.Empty, entity.Id
            );
            foreach (var member in members)
            {
                if (Guid.Parse(member.Id!) == entity.CreatorId)
                    continue;
                await backgroundTaskRepository.Create(new BackgroundTask
                {
                    PartId = entity.PartId! ?? Guid.Empty,
                    TaskId = entity.Id,
                    MemberId = Guid.Parse(member.Id!),
                    Type = (int)BackgroundTaskType.Available,
                    Timeline = model.StartTime ?? DateTime.UtcNow,
                });
            }
        }
        return isTaskCreated;
    }

    public async Task<bool> UpdateEntity(TaskModel model)
    {
        if (model.Status == 111)
        {
            var taskMembers = (await repository.GetTaskMembers(Guid.Parse(model.Id!)))
                .Select(memberMember => memberMember.MemberId);
            foreach (var member in taskMembers)
            {
                await RemoveMemberFromTask(Guid.Empty, member, Guid.Parse(model.Id!));
            }
        }
        else
        {
            var task = await repository.GetEntityById(Guid.Parse(model.Id!));
            var path = task.Path.Split("-").ToList();
            if (task.Status != model.Status && path.All(pathItem => pathItem != model.Status.ToString()))
            {
                return false;
            }
        }

        return await repository.UpdateEntity(new TaskDataModel
        {
            Id = Guid.Parse(model.Id ?? ""),
            Name = model.Name!,
            Description = model.Description!,

            StartTime = model.StartTime,
            Deadline = model.Deadline,
            ClosedAt = model.ClosedAt,
            Path = model.Path!,
            TaskTypeId = model.TaskTypeId ?? Guid.Empty,
            
            Priority = model.Priority,
            Level = model.Level,
            Status = model.Status,
        });
    }

    public Task<bool> AddToEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UnlinkEntities(Guid masterId, Guid slaveId)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<TaskModel>> GetEntitiesByQuery(string query, Guid id)
    {
        var entities = await GetEntitiesById(id);

        var queries = query.ToLower().Split(' ');

        return entities.Where(e => 
            queries.Any(q => e.Name!.Contains(q, StringComparison.CurrentCultureIgnoreCase)) &&
                                   !string.IsNullOrEmpty(e.Name) ||
            queries.Any(q => e.Description!.Contains(q, StringComparison.CurrentCultureIgnoreCase)) &&
                                   !string.IsNullOrEmpty(e.Description)
            ).ToList();
    }

    public async Task<bool> DeleteEntity(Guid id)
    {
        return await repository.DeleteEntity(id);
    }

    public async Task<ICollection<TaskHistoryModel>> GetTaskHistory(Guid taskId)
    {
        var histories = (await historyRepository.GetByTaskId(taskId))
            .Select(history => new TaskHistoryModel
            {
                InitiatorId = history.InitiatorId.ToString(),
                ActionType = (int)history.ActionType,
                Description = history.Description!,
                Name = history.Name,
                SourceStatusId = history.SourceStatusId,
                DestinationStatusId = history.DestinationStatusId,
                TaskId = history.TaskId.ToString(),
                TargetMemberId = history.TargetMemberId.ToString(),
                CreatedAt = history.CreatedAt,
            }).ToList();

        var memberCache = new Dictionary<string, MemberModel>();
    
        foreach (var history in histories)
        {
            try
            {
                if (!string.IsNullOrEmpty(history.InitiatorId))
                {
                    if (!memberCache.TryGetValue(history.InitiatorId, out var initiator))
                    {
                        initiator = await memberLogic.GetEntityById(Guid.Parse(history.InitiatorId));
                        memberCache[history.InitiatorId] = initiator;
                    }
                    history.Initiator = initiator;
                }

                if (!string.IsNullOrEmpty(history.TargetMemberId))
                {
                    if (!memberCache.TryGetValue(history.TargetMemberId, out var targetMember))
                    {
                        targetMember = await memberLogic.GetEntityById(Guid.Parse(history.TargetMemberId));
                        memberCache[history.TargetMemberId] = targetMember;
                    }
                    history.TargetMember = targetMember;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        return histories;
    }

    public async Task<bool> UpdateTask(HistoryModel? historyModel, TaskModel taskModel)
    {
        var partId = taskModel.PartId ?? Guid.Empty;
        var taskId = Guid.Parse(taskModel.Id!);
        var savedAvailableMembers = await GetAvailableMembersForTask(partId, taskId, true);
        var rawStatuses = (await partLogic.GetPartTaskStatuses(partId));
        var savedTask = await GetEntityById(Guid.Parse(taskModel.Id!));
        
        var isEntityUpdated = await UpdateEntity(taskModel);
        if (isEntityUpdated && historyModel is not null && !string.IsNullOrEmpty(historyModel.InitiatorId))
        {
            var task = await GetEntityById(Guid.Parse(taskModel.Id!));
            if (task.Status != savedTask.Status)
            {
                var historyId = Guid.NewGuid();
                await historyRepository.Create(new TaskHistory
                {
                    Id = historyId,
                    TaskId = Guid.Parse(task.Id!),
                    SourceStatusId = savedTask.Status,
                    DestinationStatusId = taskModel.Status,
                    InitiatorId = Guid.Parse(historyModel.InitiatorId),
                    Description = historyModel.Description!,
                    Name = historyModel.Name!,
                    ActionType = TaskActionType.StatusChanged
                });
                await NotifyAboutStatusUpdate(taskId, partId, task, rawStatuses, savedTask.Status, taskModel.Status, historyId);
                if (taskModel.Status < 110)
                    await NotifyAvailableMembers(Guid.Parse(task.Id!), partId, task, rawStatuses, savedAvailableMembers, historyId);
            }
        }

        if (isEntityUpdated)
        {
            var partMembers = await memberLogic.GetEntitiesById(partId);
            foreach (var member in partMembers)
            {
                await hubContext.Clients.User(member.Id!).SendAsync("ReceiveUpdate");
            }
        }
        return isEntityUpdated;
    }

    public async Task<bool> AddMemberToTask(Guid initiatorId, Guid memberId, Guid taskId, int groupId)
    {
        var task = await repository.GetEntityById(taskId);
        var statuses = 
            (await partLogic.GetPartTaskStatuses(task.PartId ?? Guid.Empty))
            .Select(status => status.Order);
        if (string.IsNullOrEmpty(task.Path))
            return false;
        var nodes = (task.Path.Split('-')).Select(int.Parse).ToList();
        if (task.Status == nodes.First())
        {
            var index = nodes.IndexOf(task.Status);
            if (index + 1 == nodes.Last())
                task.ClosedAt = DateTime.UtcNow;
            var updatedStatus = nodes[index+1];
            var nextStatusOrder = statuses
                .FirstOrDefault(status => status == updatedStatus);
            var nextStatus = (await partLogic.GetPartTaskStatuses(task.PartId ?? Guid.Empty))
                .FirstOrDefault(status => status.Order == nextStatusOrder);
            if (nextStatus!.PartRoleId != null && nextStatus.PartRoleId != Guid.Empty)
            {
                var role = (await partLogic.GetPartRoles(task.PartId ?? Guid.Empty))
                    .FirstOrDefault(role => role.Id == nextStatus!.PartRoleId);
                var memberStatuses = 
                    (await partLogic.GetPartMemberRoles(task.PartId ?? Guid.Empty, memberId))
                    .Select(status => status.Id);
                if (!memberStatuses.Contains(role!.Id))
                    return false;
            }
        }
        
        var isMemberAddedToTask = await repository.AddToEntity(memberId, taskId);

        if (isMemberAddedToTask)
        {
            var historyId = Guid.NewGuid();
            await historyRepository.Create(new TaskHistory
            {
                Id = historyId,
                TaskId = taskId,
                SourceStatusId = task.Status,
                DestinationStatusId = null,
                InitiatorId = initiatorId,
                Description = "",
                Name = "",
                ActionType = TaskActionType.MemberAdded,
                TargetMemberId = memberId,
            });
      
            var members = await GetTaskMembers(taskId);
            var currentMember = members.FirstOrDefault(member => member.Id == memberId.ToString());
            foreach (var member in members)
            {
                if (member.Id == initiatorId.ToString())
                    continue;
                
                await backgroundTaskRepository.Create(new BackgroundTask
                {
                    TaskId = taskId,
                    PartId = task.PartId ?? Guid.Empty,
                    MemberId = Guid.Parse(member.Id!),
                    HistoryId = historyId,
                    Timeline = DateTime.UtcNow,
                    Type = (int)BackgroundTaskType.Added,
                    Message = currentMember!.FirstName + " " + currentMember.LastName,
                });
            }
        }
        
        return isMemberAddedToTask;
    }

    public async Task<bool> RemoveMemberFromTask(Guid initiatorId, Guid memberId, Guid taskId)
    {
        var isMemberRemoved = await repository.RemoveFromEntity(memberId, taskId);
        if (isMemberRemoved && initiatorId != Guid.Empty)
        {
            var historyId = Guid.NewGuid();
            await historyRepository.Create(new TaskHistory
            {
                Id = historyId,
                TaskId = taskId,
                SourceStatusId = null,
                DestinationStatusId = null,
                InitiatorId = initiatorId,
                Description = "",
                Name = "",
                ActionType = TaskActionType.Reassigned,
                TargetMemberId = memberId,
            });
            
            var members = await GetTaskMembers(taskId);
            members.Add(new MemberModel
            {
                Id = memberId.ToString(),
            });
            foreach (var member in members)
            {
                await backgroundTaskRepository.Create(new BackgroundTask
                {
                    HistoryId = historyId,
                    TaskId = taskId,
                    MemberId = Guid.Parse(member.Id!),
                    Type = (int)BackgroundTaskType.Removed,
                    Timeline = DateTime.UtcNow
                });
            }
        }
        
        return isMemberRemoved;
    }

    public async Task<bool> ChangeTaskStatus(HistoryModel historyModel, Guid taskId, bool forward)
    {
        //TODO: Нужно это разобрать на отдельные методы
        var task = await repository.GetEntityById(taskId);
        var partId = task.PartId ?? Guid.Empty;
        var savedAvailableMembers = await GetAvailableMembersForTask(partId, taskId, true);
        
        var rawStatuses = (await partLogic.GetPartTaskStatuses(task.PartId ?? Guid.Empty));
        var statuses = rawStatuses
            .OrderBy(status => status.Order)
            .Select(status => status.Order)
            .SkipLast(1).ToList();
        if (string.IsNullOrEmpty(task.Path))
            return false;
        var nodes = (task.Path.Split('-'))
            .Select(int.Parse)
            .ToList();
        bool isPathValid = true;
        foreach (var node in nodes.ToList())
        {
            if (!statuses!.Contains(node))
            {
                isPathValid = false;
                nodes.Remove(node);
            }
        }
        if (!isPathValid)
        {
            task.Status = statuses[0];
            task.Path = string.Join('-', nodes);
        }
        
        if (task.Status >= nodes.Last())
            return false;
        if (nodes.Any(node => !statuses.Contains(node)))
            return false;
        var index = nodes.IndexOf(task.Status);
        if (forward && index == nodes.Count - 1) 
            return false;
        if (!forward && index == 0)
            return false;
        var targetIndex = forward ? index + 1 : index - 1;
        if (targetIndex == nodes.Count - 1)
            task.ClosedAt = DateTime.UtcNow;
        
        var sourceStatus = task.Status;
        task.Status = nodes[targetIndex];
        
        var isEntityUpdated = await UpdateEntity(ConvertToLogicModel(task));
        
        if (isEntityUpdated && !string.IsNullOrEmpty(historyModel.InitiatorId))
        {
            var historyId = Guid.NewGuid();
            await historyRepository.Create(new TaskHistory
            {
                Id = historyId,
                TaskId = taskId,
                SourceStatusId = sourceStatus,
                DestinationStatusId = task.Status,
                InitiatorId = Guid.Parse(historyModel.InitiatorId),
                Description = historyModel.Description!,
                Name = historyModel.Name!,
                ActionType = TaskActionType.StatusChanged,
            });
            await NotifyAboutStatusUpdate(taskId, partId, 
                ConvertToLogicModel(task), rawStatuses, sourceStatus, task.Status, historyId);
            await NotifyAvailableMembers(taskId, partId, 
                ConvertToLogicModel(task), rawStatuses, savedAvailableMembers, historyId);
        }
        
        return isEntityUpdated;
    }

    private async Task NotifyAboutStatusUpdate(Guid taskId, Guid partId, TaskModel task, 
        ICollection<PartTaskStatus> rawStatuses, int sourceStatusId, int destinationStatusId, Guid historyId)
    {
        var taskMembers = await repository.GetTaskMembers(taskId);
        var sourceStatus = rawStatuses
            .Where(status => status.Order == sourceStatusId)
            .Select(status => status.Name).First();
        var destinatinoStatus = rawStatuses
            .Where(status => status.Order == destinationStatusId)
            .Select(status => status.Name).First();
        
        foreach (var member in taskMembers)
        {
            await backgroundTaskRepository.Create(new BackgroundTask
            {
                TaskId = taskId,
                PartId = partId,
                MemberId = member.MemberId,
                HistoryId = historyId,
                Message = !string.IsNullOrEmpty(sourceStatus) && !string.IsNullOrEmpty(destinatinoStatus)
                    ? $"Переведена из [{sourceStatus}] в [{destinatinoStatus}]"
                    : "обновлена",
                Type = (int)BackgroundTaskType.StatusUpdate,
                Timeline = DateTime.UtcNow
            });
        }
        
    }
    
    private async Task NotifyAvailableMembers(
        Guid taskId, Guid partId, TaskModel task, 
        ICollection<PartTaskStatus> rawStatuses, ICollection<MemberModel> savedAvailableMembers, Guid historyId)
    {
        var taskMembers = await GetTaskMembers(taskId);
        int accessCounter = 0;
            
        foreach (var taskMember in taskMembers)
        {
            var memberId = Guid.Parse(taskMember.Id!);

            if (await HasAccessToTask(memberId, partId, task, rawStatuses))
                accessCounter++;
        }

        var availableMembers = await GetAvailableMembersForTask(
            partId, Guid.Parse(task.Id!), true);
        
        var filteredMembers = new List<MemberModel>();
        if (accessCounter == 0)
        {
            filteredMembers.AddRange(availableMembers);
        }
        else if (savedAvailableMembers.Count != availableMembers.Count)
        {
            filteredMembers.AddRange(availableMembers
                .Where(member => savedAvailableMembers.All(savedMember => savedMember.Id != member.Id)));
        }
        foreach (var member in filteredMembers)
        {
            await backgroundTaskRepository.Create(new BackgroundTask
            {
                TaskId = taskId,
                PartId = partId,
                MemberId = Guid.Parse(member.Id!),
                HistoryId = historyId,
                Type = (int)BackgroundTaskType.Available,
                Timeline = DateTime.UtcNow
            });
        }
    }
    
    public async Task<ICollection<TaskModel>> GetFreeTasks(Guid partId)
    {
        var tasks = await repository.GetFreeTasks(partId);

        return tasks.Select(task => ConvertToLogicModel(task!)).ToList();
    }

    public async Task<ICollection<TaskModel>> GetAvailableTasks(Guid memberId, Guid partId)
    {
        var roles = await partLogic.GetPartMemberRoles(partId, memberId);
        var statuses = await partLogic.GetPartTaskStatuses(partId);
        var tasks = await GetEntitiesById(partId);
        
        var availableTasks = new List<TaskModel>();
        foreach (var task in tasks)
        {
            var targetStatus = task.Status.ToString();
            if (task.Status == 0)
            {
                var array = task.Path.Split('-');
                var index = Array.IndexOf(array, task.Status.ToString());
                targetStatus = index >= 0 && index < array.Length - 1 
                    ? array[index + 1] 
                    : null;
            }
            var taskMembers = await GetTaskMembers(Guid.Parse(task.Id!));
            
            int accessCounter = 0;
            foreach (var taskMember in taskMembers)
            {
                if (await HasAccessToTask(Guid.Parse(taskMember.Id!), partId, task, statuses))
                    accessCounter++;
            }
            if (accessCounter != 0)
                continue;
            var taskStatus = statuses
                .FirstOrDefault(status => status.Order.ToString() == targetStatus);
            if (taskStatus == null)
                continue;
            if (taskStatus!.PartRoleId is null || roles.Any(role => role.Id == taskStatus!.PartRoleId))
            {
                availableTasks.Add(task);
            }
        }
        
        return availableTasks;
    }

    public async Task<ICollection<MemberModel>> GetAvailableMembersForTask(Guid partId, Guid taskId, bool includeExecutors = false)
    {
        var task = await GetEntityById(taskId);
        var statuses = await partLogic.GetPartTaskStatuses(partId);
        var members = (await memberLogic.GetMembersFromPart(partId));
        var taskMembers = await GetTaskMembers(Guid.Parse(task.Id!));
        
        var availableMembers = new List<MemberModel>();
        foreach (var member in members)
        {
            if ((includeExecutors || taskMembers.All(taskMember => taskMember.Id != member.Id)) 
                && await HasAccessToTask(Guid.Parse(member.Id!), partId, task, statuses))
            {
                availableMembers.Add(await memberLogic.GetEntityById(Guid.Parse(member.Id!)));
            }
        }

        return availableMembers;
    }

    private async Task<bool> HasAccessToTask(
        Guid memberId, Guid partId, TaskModel task, ICollection<PartTaskStatus> taskStatuses)
    {
        var targetStatus = task.Status.ToString();
        if (task.Status == 0)
        {
            var array = task.Path!.Split('-');
            var index = Array.IndexOf(array, task.Status.ToString());
            targetStatus = index >= 0 && index < array.Length - 1 
                ? array[index + 1] 
                : null;
        }
        
        var taskStatus = taskStatuses
            .FirstOrDefault(status => status.Order.ToString() == targetStatus);

        var roles = await partLogic.GetPartMemberRoles(partId, memberId);
        var isMemberAvailableForThisTask =
            (taskStatus is null 
             || taskStatus!.PartRoleId is null 
             || roles.Any(role => role.Id == taskStatus!.PartRoleId));
        return isMemberAvailableForThisTask;
    }
    public async Task<ICollection<TaskModel>> GetMemberTasks(Guid memberId)
    {
        var tasks = await repository.GetMemberTasks(memberId);

        var updatedTasks = tasks.Select(task => ConvertToLogicModel(task!)).ToList();
        
        foreach (var task in updatedTasks)
        {
            var partId = task.PartId ?? Guid.Empty;
            if (partId == Guid.Empty)
                continue;
            var statuses = await partLogic.GetPartTaskStatuses(partId);

            task.IsAvailable = await HasAccessToTask(memberId, partId, task, statuses);
        }
        return updatedTasks;
    }

    public async Task<ICollection<MemberModel>> GetTaskMembers(Guid taskId)
    {
        var membersIds = await repository.GetTaskMembersIds(taskId);
        var memberCollection = new List<MemberModel>();
        
        foreach (var memberId in membersIds)
        {
            var member = await memberLogic.GetEntityById(memberId);
            memberCollection.Add(member);
        }
        return memberCollection;
    }
    
    private TaskModel ConvertToLogicModel(TaskDataModel model)
    {
        return new TaskModel
        {
            Id = model.Id.ToString(),
            
            Name = model.Name,
            Description = model.Description,
            
            CreatorId = model.CreatorId,
            PartId = model.PartId,
            
            StartTime = model.StartTime,
            Deadline = model.Deadline,
            ClosedAt = model.ClosedAt,
            Path = model.Path,
            TaskTypeId = model.TaskTypeId,
            
            Level = model.Level,
            Status = model.Status,
            Priority = model.Priority,
        };
    }
}