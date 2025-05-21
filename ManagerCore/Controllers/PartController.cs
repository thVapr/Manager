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
    [HttpGet]
    [Route("get_privileges")]
    public async Task<IActionResult> GetPrivileges(Guid memberId, Guid partId)
    {
        return Ok(await partLogic.GetPrivileges(memberId, partId));
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("get_part_statuses")]
    public async Task<IActionResult> GetPartTaskStatuses(Guid partId)
    {
        return Ok(await partLogic.GetPartTaskStatuses(partId));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader, true])]
    [HttpPost]
    [Route("statuses/create")]
    public async Task<IActionResult> CreatePartTaskStatus([FromBody] PartTaskStatusModel model)
    {
        if (await partLogic.AddPartTaskStatus(model))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpPut]
    [Route("statuses/update")]
    public async Task<IActionResult> UpdatePartTaskStatus([FromBody] PartTaskStatusModel model)
    {
        if (await partLogic.ChangePartTaskStatus(model))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpDelete]
    [Route("statuses/remove")]
    public async Task<IActionResult> RemovePartTaskStatus(string partId, string partTaskStatusId)
    {
        if (await partLogic.RemovePartTaskStatus(Guid.Parse(partId), Guid.Parse(partTaskStatusId)))
            return Ok();
        return BadRequest();
    }
    
    [HttpGet]
    [Route("check_privileges")]
    public async Task<IActionResult> CheckPrivileges(Guid userId, Guid partId, int privilege)
    {
        var isMemberHasPrivilege = await partLogic.IsUserHasPrivileges(userId, partId, privilege);
        return Ok(isMemberHasPrivilege);
    }
    
    [HttpGet]
    [Route("all_accessible")]
    public async Task<IActionResult> GetAll()
    {
        if (User.IsInRole(RoleConstants.Admin))
            return Ok(await partLogic.GetEntities());
        var userId = User.FindFirst("id")?.Value;
        return Ok(await partLogic.GetAllAccessibleParts(Guid.Parse(userId!)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpPost]
    [Route("update_hierarchy")]
    public async Task<IActionResult> UpdateHierarchy(List<PartModel> models)
    {
        return Ok(await partLogic.UpdateHierarchy(models));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string partId)
    {
        return Ok(await partLogic.GetEntitiesById(Guid.Parse(partId)));
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string partId)
    {
        return Ok(await partLogic.GetEntityById(Guid.Parse(partId)));
    }
    
    [HttpGet]
    [Route("get_types")]
    public async Task<IActionResult> GetTypes()
    {
        return Ok(await partLogic.GetPartTypes());
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader, true])]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(PartModel model)
    {
        var userId = User.FindFirst("id")?.Value;
        if (await partLogic.CreatePart(Guid.Parse(userId!),model))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("change_privilege")]
    public async Task<IActionResult> ChangePrivilege([FromBody] PrivilegeChangeRequest request)
    {
        if (await partLogic.ChangePrivilege(Guid.Parse(request.MemberId), Guid.Parse(request.PartId), request.Privilege))
            return Ok();

        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddMemberToPart([FromBody] PartMembersModel model)
    {
        if (await partLogic.AddToEntity(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveMemberFromPart([FromBody] PartMembersModel model)
    {
        if (await partLogic.RemoveFromEntity(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdatePart([FromBody] PartModel model)
    {
        if (await partLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpPost]
    [Route("link")]
    public async Task<IActionResult> LinkParts(string masterId, string slaveId)
    {
        if (await partLogic.LinkEntities(Guid.Parse(masterId), Guid.Parse(slaveId)))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpPost]
    [Route("move")]
    public async Task<IActionResult> MovePart(string masterId, string slaveId)
    {
        if (await partLogic.UnlinkEntities(Guid.Parse(masterId), Guid.Parse(slaveId)))
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Leader])]
    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeletePart(string partId)
    {
        if (await partLogic.DeleteEntity(Guid.Parse(partId)))
            return Ok();
        return BadRequest();
    }
}