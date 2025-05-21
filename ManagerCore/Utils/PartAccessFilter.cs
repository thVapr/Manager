using ManagerData.Constants;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManagerCore.Utils;

public class PartAccessFilter(IPartLogic partLogic, int requiredLevel, bool isNotZeroLevelCreationAccess = false) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        
        if (!user.IsInRole(RoleConstants.Admin) &&
            !user.IsInRole(RoleConstants.SpaceOwner) && !isNotZeroLevelCreationAccess
        )
        {
            var userId = FilterHelper.GetUserId(context.HttpContext.User);
            var partIds = FilterHelper.GetPartIds(context);
            if (!partIds.Any())
            {
                context.Result = new ForbidResult();
                return;
            }
            foreach (var partId in partIds)
            {
                if (await partLogic.IsUserHasPrivileges(userId, partId, requiredLevel))
                    continue;
                context.Result = 
                    new ForbidResult("Недостаточно привилегий для доступа к данной сущности");
                return;
            }
        }

        await next();
    }
}