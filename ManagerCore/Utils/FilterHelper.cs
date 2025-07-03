using ManagerCore.Models;
using ManagerLogic.Models;

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManagerCore.Utils;

public static class FilterHelper
{
    public static Guid? GetMemberId(ActionExecutingContext context)
    {
        var memberId = (context.ActionArguments.TryGetValue("memberId", out var id) 
            ? id : null)
            ?? context.ActionArguments.Values.OfType<MemberTasks>().FirstOrDefault()?.MemberId;

        return Guid.TryParse(memberId?.ToString(), out var parsedId)
            ? parsedId : null;
    }
    
    public static Guid GetUserId(ClaimsPrincipal user)
    {
        return Guid.TryParse(user.FindFirst("id")?.Value, out var id)
            ? id
            : Guid.Empty;
    }

    public static Guid? GetTaskId(ActionExecutingContext context)
    {
        var taskId = ((context.ActionArguments.TryGetValue("taskId", out var id) 
            ? id : null) ?? context.ActionArguments.Values.OfType<MemberTasks>().FirstOrDefault()?.TaskId)
                         ?? context.ActionArguments.Values.OfType<TaskMessageModel>().FirstOrDefault()?.TaskId;
        return Guid.TryParse((string?)taskId, out var parsedId) ? parsedId : null;
    }
    
    public static List<Guid> GetPartIds(ActionExecutingContext context)
    {
        var partId = ((((context.ActionArguments.TryGetValue("partId", out var id)
               ? id : null)
           ?? (context.ActionArguments.TryGetValue("masterId", out id)
               ? id : null)
            ?? context.ActionArguments.Values.OfType<PartModel>().FirstOrDefault()?.MainPartId)
            ?? context.ActionArguments.Values.OfType<UpdateTaskRequest>().FirstOrDefault()?.Task.PartId)
            ?? context.ActionArguments.Values.OfType<TaskModel>().FirstOrDefault()?.PartId)
            ?? context.ActionArguments.Values.OfType<IPartAllocationModel>().FirstOrDefault()?.PartId;
        
        var list = new List<string>();
        if (partId != null)
            list.Add(partId.ToString()!);

        var partIds = context.ActionArguments.Values.OfType<List<PartModel>>().FirstOrDefault();
        if (partIds != null) 
            list.AddRange(partIds.Select(potentialPartId => potentialPartId.Id!));

        return list.Select(l => Guid.TryParse(l, out var parsedId)
            ? parsedId
            : Guid.Empty).ToList();
    }
}