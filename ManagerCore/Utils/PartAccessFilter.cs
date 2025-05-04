using ManagerLogic.Models;
using ManagerData.Constants;
using System.Security.Claims;
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
            var userId = GetUserId(context.HttpContext.User);
            var partIds = GetPartIds(context);

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

    private List<Guid> GetPartIds(ActionExecutingContext context)
    {
        var partId = (context.ActionArguments.TryGetValue("partId", out var id) 
            ? id : null)
                ?? (context.ActionArguments.TryGetValue("masterId", out id)
            ? id : null)
                ?? (context.ActionArguments.Values.OfType<PartModel>().FirstOrDefault()?.MainPartId);
        
        var list = new List<string>();
        if (partId != null)
            list.Add(partId.ToString()!);

        var partIds = context.ActionArguments.Values.OfType<List<PartModel>>().ToList();
        foreach (var potentialPartId in partIds)
        {
            list.Add(potentialPartId.ToString()!);
        }
        
        return list.Select(l => Guid.TryParse(l, out var parsedId)
            ? parsedId
            : Guid.Empty).ToList();
    }
}