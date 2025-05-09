using ManagerCore.Models;
using ManagerCore.Utils;
using ManagerLogic;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts/roles")]
[Authorize]
public class PartRoleController(IPartLogic partLogic) : ControllerBase
{
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("add_role")]
    public async Task<IActionResult> AddRoleToPart([FromBody] PartRoleModel model)
    {
        if (await partLogic.AddRoleToPart(Guid.Parse(model.PartId!), model.Name, model.Description))
            return Ok(true);
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("add_member")]
    public async Task<IActionResult> AddMemberToRoleInPart([FromBody] PartMemberRoleRequest model)
    {
        if (await partLogic
                .AddMemberToRole(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!), Guid.Parse(model.RoleId!)))
            return Ok(true);
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("remove_member")]
    public async Task<IActionResult> RemoveMemberFromRoleInPart([FromBody] PartMemberRoleRequest model)
    {
        if (await partLogic
                .RemoveMemberFromRole(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!), Guid.Parse(model.RoleId!)))
            return Ok(true);
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("get_roles")]
    public async Task<IActionResult> GetRoles(string partId)
    {
        var roles = await partLogic.GetPartRoles(Guid.Parse(partId));
        if (roles.Any())
            return Ok(roles);
        return BadRequest($"У сущности с id {partId} нет доступных ролей");
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("get_member_roles")]
    public async Task<IActionResult> GetMemberRoles(string partId, string memberId)
    {
        var roles = await partLogic.GetPartMemberRoles(Guid.Parse(partId), Guid.Parse(memberId));
        if (roles.Any())
            return Ok(roles);
        return BadRequest($"У пользователя с id {memberId} нет доступных ролей");
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("remove_role")]
    public async Task<IActionResult> RemoveRoleFromPart([FromBody] PartRoleModel model)
    {
        if (await partLogic.RemoveRoleFromPart(Guid.Parse(model.PartId), Guid.Parse(model.RoleId!)))
            return Ok();
        return BadRequest();
    }
}