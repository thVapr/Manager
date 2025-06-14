using ManagerLogic;
using ManagerCore.Utils;
using ManagerCore.Models;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts/{partId}/roles")]
[Authorize]
public class PartRoleController(IPartLogic partLogic) : ControllerBase
{
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> AddRoleToPart(string partId, [FromBody] AddToPartModel model)
    {
        if (await partLogic.AddRoleToPart(Guid.Parse(partId), model.Name!, model.Description ?? string.Empty))
            return Ok(true);
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("{roleId}/members/{memberId}")]
    public async Task<IActionResult> AddMemberToRoleInPart(string partId, string roleId, string memberId)
    {
        if (await partLogic
                .AddMemberToRole(Guid.Parse(partId),
                    Guid.Parse(memberId), Guid.Parse(roleId)))
            return Ok(true);
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpDelete]
    [Route("{roleId}/members/{memberId}")]
    public async Task<IActionResult> RemoveMemberFromRoleInPart(string partId, string roleId, string memberId)
    {
        if (await partLogic
                .RemoveMemberFromRole(Guid.Parse(partId), Guid.Parse(memberId), Guid.Parse(roleId)))
            return Ok(true);
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetRoles(string partId)
    {
        var roles = await partLogic.GetPartRoles(Guid.Parse(partId));
        if (roles.Any())
            return Ok(roles);
        return BadRequest($"У сущности с id {partId} нет доступных ролей");
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("members/{memberId}")]
    public async Task<IActionResult> GetMemberRoles(string partId, string memberId)
    {
        var roles = await partLogic.GetPartMemberRoles(Guid.Parse(partId), Guid.Parse(memberId));
        if (roles.Any())
            return Ok(roles);
        return BadRequest($"У пользователя с id {memberId} нет доступных ролей");
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpDelete]
    [Route("{roleId}")]
    public async Task<IActionResult> RemoveRoleFromPart(string partId, string roleId)
    {
        if (await partLogic.RemoveRoleFromPart(Guid.Parse(partId), Guid.Parse(roleId)))
            return Ok();
        return BadRequest();
    }
}