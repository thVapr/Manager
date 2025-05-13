using ManagerLogic;
using ManagerLogic.Models;
using ManagerData.Constants;
using System.Security.Claims;
using ManagerCore.Models;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManagerCore.Utils;

public class TaskAccessFilter(IPartLogic partLogic, ITaskLogic taskLogic) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        
        if (!user.IsInRole(RoleConstants.Admin))
        {
            var userId = FilterHelper.GetUserId(context.HttpContext.User);
            var taskId = FilterHelper.GetTaskId(context);
            if (!taskId.HasValue || taskId == Guid.Empty)
                return;
            var task = await taskLogic.GetEntityById(taskId.Value);
            var partId = task.PartId;
            if (userId != task.CreatorId && partId.HasValue)
            {
                var userHasPrivileges = await partLogic.IsUserHasPrivileges(userId, partId.Value, (int)AccessLevel.Leader);
                userHasPrivileges = userHasPrivileges || await partLogic.IsUserHasPrivileges(userId, partId.Value, (int)AccessLevel.Control);
                if (!userHasPrivileges)
                {
                    var role = await partLogic.GetPartMemberRoles(partId.Value, userId);
                    var status = (await partLogic.GetPartTaskStatuses(partId.Value))
                        .FirstOrDefault(status => status.Order == task.Status);

                    var isUserNotInRole = status?.PartRoleId != null
                                 && status.PartRoleId != Guid.Empty
                                 && role.All(partRole => partRole.Id != status.PartRoleId);
                    if (isUserNotInRole)
                        return;
                }
            }
        }

        await next();
    }
}