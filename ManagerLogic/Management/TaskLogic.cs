using System.Collections;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Models;

namespace ManagerLogic.Management;

public class TaskLogic(ITaskRepository repository, IPartLogic partLogic, IMemberLogic memberLogic, IHistoryRepository historyRepository) : ITaskLogic
{
    public async Task<TaskModel> GetEntityById(Guid id)
    {
        var task = await repository.GetEntityById(id);
        return ConvertToLogicModel(task!);
    }

    public async Task<ICollection<TaskModel>> GetEntities()
    {
        var entities = await repository.GetEntities();

        if (entities == null) return [];

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
        return await repository.CreateEntity((Guid)model.PartId!, entity);
    }

    public async Task<bool> UpdateEntity(TaskModel model)
    {
        if (model.Status == 111)
        {
            var taskMembers = (await repository.GetTaskMembers(Guid.Parse(model.Id!)))
                .Select(memberMember => memberMember.MemberId);
            foreach (var member in taskMembers)
            {
                await RemoveMemberFromTask(member, Guid.Parse(model.Id!));
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

    public async Task<bool> AddMemberToTask(Guid memberId, Guid taskId, int groupId)
    {
        var task = await repository.GetEntityById(taskId);
        var statuses = (await partLogic.GetPartTaskStatuses(task.PartId ?? Guid.Empty)).Select(status => status.Order);
        if (string.IsNullOrEmpty(task.Path))
            return false;
        var nodes = (task.Path.Split('-')).Select(int.Parse).ToList();
        if (task.Status == nodes.First())
        {
            var index = nodes.IndexOf(task.Status);
            if (index + 1 == nodes.Last())
                task.ClosedAt = DateTime.Now;
            var updatedStatus = nodes[index+1];
            var nextStatusOrder = statuses
                .FirstOrDefault(status => status == updatedStatus);
            var nextStatus = (await partLogic.GetPartTaskStatuses(task.PartId ?? Guid.Empty))
                .FirstOrDefault(status => status.Order == nextStatusOrder);
            if (nextStatus!.PartRoleId != null && nextStatus.PartRoleId != Guid.Empty)
            {
                var role = (await partLogic.GetPartRoles(task.PartId ?? Guid.Empty))
                    .FirstOrDefault(role => role.Id == nextStatus!.PartRoleId);
                var memberStatuses = (await partLogic.GetPartMemberRoles(task.PartId ?? Guid.Empty, memberId))
                    .Select(status => status.Id);
                if(!memberStatuses.Contains(role!.Id))
                    return false;
            }
            
            await repository.UpdateEntity(new TaskDataModel
            {
                Id = taskId,
                Status = updatedStatus,
            });
        }

        return await repository.AddToEntity(memberId, taskId);
    }

    public async Task<bool> RemoveMemberFromTask(Guid employeeId, Guid taskId)
    {
        return await repository.RemoveFromEntity(employeeId, taskId);
    }

    public async Task<bool> ChangeTaskStatus(HistoryModel historyModel, Guid taskId)
    {
        //TODO: Нужно это разобрать на отдельные методы
        var task = await repository.GetEntityById(taskId);
        var statuses = (await partLogic.GetPartTaskStatuses(task.PartId ?? Guid.Empty))
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
        if (index == nodes.Last()) 
            return false;
        if (index + 1 == nodes.Last())
            task.ClosedAt = DateTime.Now;
        
        var taskMembers = (await repository.GetTaskMembers(taskId))
            .Where(taskMember => taskMember.ParticipationPurpose == 1)
            .Select(memberMember => memberMember.MemberId);
        var memberRoles = new Dictionary<Guid, ICollection<PartRole>>();
        foreach (var memberId in taskMembers)
        {
            memberRoles[memberId] = await partLogic.GetPartMemberRoles(task.PartId ?? Guid.Empty, memberId);
        }
        var nextStatus = (await partLogic.GetPartTaskStatuses(task.PartId ?? Guid.Empty))
            .FirstOrDefault(status => status.Order == nodes[index+1]);
        if (nextStatus!.PartRoleId != null && nextStatus.PartRoleId != Guid.Empty)
        {
            foreach (var memberRole in memberRoles)
            {
                if (memberRole.Value.All(role => role.Id != nextStatus.PartRoleId.Value))
                    await RemoveMemberFromTask(memberRole.Key, taskId);
            }
        }

        if (!string.IsNullOrEmpty(historyModel.InitiatorId))
        {
            await historyRepository.Create(new TaskHistory
            {
                TaskId = taskId,
                SourceStatusId = task.Status,
                DestinationStatusId = nodes[index+1],
                InitiatorId = Guid.Parse(historyModel.InitiatorId),
                Description = historyModel.Description!,
                Name = historyModel.Name!,
            });
        }
        task.Status = nodes[index+1];
        
        return await UpdateEntity(ConvertToLogicModel(task));
    }

    public async Task<ICollection<TaskModel>> GetFreeTasks(Guid projectId)
    {
        var tasks = await repository.GetFreeTasks(projectId);

        return tasks.Select(task => ConvertToLogicModel(task!)).ToList();
    }

    public async Task<ICollection<TaskModel>> GetMemberTasks(Guid employeeId)
    {
        var tasks = await repository.GetMemberTasks(employeeId);

        return tasks.Select(task => ConvertToLogicModel(task!)).ToList();
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
            
            Level = model.Level,
            Status = model.Status,
            Priority = model.Priority,
        };
    }
}