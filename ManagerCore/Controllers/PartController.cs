using ManagerLogic;
using ManagerCore.Utils;
using ManagerCore.Models;
using ManagerLogic.Models;
using ManagerData.Constants;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts")]
[Authorize]
public class PartController(IPartLogic partLogic) : ControllerBase
{
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("{partId}/statuses")]
    public async Task<IActionResult> GetPartTaskStatuses(Guid partId)
    {
        return Ok(await partLogic.GetPartTaskStatuses(partId));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader, true])]
    [HttpPost]
    [Route("{partId}/statuses")]
    public async Task<IActionResult> CreatePartTaskStatus([FromBody] PartTaskStatusModel model)
    {
        if (await partLogic.AddPartTaskStatus(model))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpPut]
    [Route("{partId}/statuses/{partTaskStatusId}")]
    public async Task<IActionResult> UpdatePartTaskStatus([FromBody] PartTaskStatusModel model)
    {
        if (await partLogic.ChangePartTaskStatus(model))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpDelete]
    [Route("{partId}/statuses/{partTaskStatusId}")]
    public async Task<IActionResult> RemovePartTaskStatus(string partId, string partTaskStatusId)
    {
        if (await partLogic.RemovePartTaskStatus(Guid.Parse(partId), Guid.Parse(partTaskStatusId)))
            return Ok();
        return BadRequest();
    }
    
    [HttpGet]
    [Route("{partId}/privileges/{memberId}")]
    public async Task<IActionResult> GetPrivileges(Guid memberId, Guid partId)
    {
        return Ok(await partLogic.GetPrivileges(memberId, partId));
    }
    
    [HttpGet]
    [Route("{partId}/privileges/{userId}/check")]
    public async Task<IActionResult> CheckPrivileges(Guid userId, Guid partId, int privilege)
    {
        var isMemberHasPrivilege = await partLogic.IsUserHasPrivileges(userId, partId, privilege);
        return Ok(isMemberHasPrivilege);
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("{partId}/privileges/{memberId}")]
    public async Task<IActionResult> ChangePrivilege([FromBody] PrivilegeChangeRequest request, string partId, string memberId)
    {
        if (await partLogic.ChangePrivilege(Guid.Parse(memberId), Guid.Parse(partId), request.Privilege))
            return Ok();

        return BadRequest();
    }
    
    [HttpGet]
    [Route("accessible")]
    public async Task<IActionResult> GetAll()
    {
        if (User.IsInRole(RoleConstants.Admin))
            return Ok(await partLogic.GetEntities());
        var userId = User.FindFirst("id")?.Value;
        return Ok(await partLogic.GetAllAccessibleParts(Guid.Parse(userId!)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpPost]
    [Route("hierarchy")]
    public async Task<IActionResult> UpdateHierarchy(List<PartModel> models)
    {
        return Ok(await partLogic.UpdateHierarchy(models));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("{partId}/all")]
    public async Task<IActionResult> GetAll(string partId)
    {
        return Ok(await partLogic.GetEntitiesById(Guid.Parse(partId)));
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("{partId}")]
    public async Task<IActionResult> GetModel(string partId)
    {
        return Ok(await partLogic.GetEntityById(Guid.Parse(partId)));
    }
    
    [HttpGet]
    [Route("types")]
    public async Task<IActionResult> GetTypes()
    {
        return Ok(await partLogic.GetPartTypes());
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader, true])]
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateModel(PartModel model)
    {
        var userId = User.FindFirst("id")?.Value;
        if (await partLogic.CreatePart(Guid.Parse(userId!),model))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [HttpPost]
    [Route("{partId}/members/")]
    public async Task<IActionResult> AddMemberToPart(string partId, [FromBody] PartMembersModel model)
    {
        if (await partLogic.AddToEntity(Guid.Parse(partId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpDelete]
    [Route("{partId}/members/{memberId}")]
    public async Task<IActionResult> RemoveMemberFromPart(string partId, string memberId)
    {
        if (await partLogic.RemoveFromEntity(Guid.Parse(partId!), Guid.Parse(memberId!)))
            return Ok();

        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpPut]
    [Route("")]
    public async Task<IActionResult> UpdatePart([FromBody] PartModel model)
    {
        if (await partLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpPost]
    [Route("{masterId}/links")]
    public async Task<IActionResult> LinkParts(string masterId, string slaveId)
    {
        if (await partLogic.LinkEntities(Guid.Parse(masterId), Guid.Parse(slaveId)))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpDelete]
    [Route("{masterId}/links")]
    public async Task<IActionResult> MovePart(string masterId, string slaveId)
    {
        if (await partLogic.UnlinkEntities(Guid.Parse(masterId), Guid.Parse(slaveId)))
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpDelete]
    [Route("{partId}")]
    public async Task<IActionResult> DeletePart(string partId)
    {
        if (await partLogic.DeleteEntity(Guid.Parse(partId)))
            return Ok();
        return BadRequest();
    }
}