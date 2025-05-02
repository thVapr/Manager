using System.Security.Claims;
using ManagerData.Constants;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManagerCore.Utils;

public class PartAccessFilter(IPartLogic partLogic, int requiredLevel, bool isZeroLevelCreationAccess = false) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var role = GetRole(context.HttpContext.User);
        
        if (string.CompareOrdinal(role, RoleConstants.Admin) != 0
            && string.CompareOrdinal(role, RoleConstants.SpaceOwner) != 0 && isZeroLevelCreationAccess)
        {
            var userId = GetUserId(context.HttpContext.User);
            var partId = GetPartId(context);
            
            if (!await partLogic.IsUserHasPrivileges(userId, partId, requiredLevel))
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        await next();
    }

    private string? GetRole(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value;
    }

    private Guid GetUserId(ClaimsPrincipal user)
    {
        return Guid.TryParse(user.FindFirst("id")?.Value, out var id)
            ? id
            : Guid.Empty;
    }

    private Guid GetPartId(ActionExecutingContext context)
    {
        var partId = context.ActionArguments.TryGetValue("id", out var id) 
            ? id :
            string.Empty;
        return Guid.TryParse(partId?.ToString(), out var parsedId)
            ? parsedId
            : Guid.Empty;
    }
}