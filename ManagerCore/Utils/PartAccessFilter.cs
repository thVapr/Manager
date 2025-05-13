using ManagerLogic.Models;
using ManagerData.Constants;
using System.Security.Claims;
using ManagerCore.Models;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManagerCore.Utils;

public class PartAccessFilter(IPartLogic partLogic, int requiredLevel, bool isZeroLevelCreationAccess = false) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        
        if (!user.IsInRole(RoleConstants.Admin) ||
            !user.IsInRole(RoleConstants.SpaceOwner) && isZeroLevelCreationAccess
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
                context.Result = new ForbidResult();
                return;
            }
        }

        await next();
    }
}