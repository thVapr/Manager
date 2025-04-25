using ManagerCore.Models;
using ManagerCore.Utils;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts")]
[Authorize]
public class PartController(IPartLogic partLogic, IHttpContextAccessor contextAccessor) : ControllerBase
{
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string id)
    {
        return Ok(await partLogic.GetEntitiesById(Guid.Parse(id)));
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [1])]
    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await partLogic.GetEntityById(Guid.Parse(id)));
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(PartModel model)
    {
        if (await partLogic.CreateEntity(model))
            return Ok();

        return BadRequest();
    }
    
    [HttpPost]
    [Route("change_privilege")]
    public async Task<IActionResult> ChangePrivilege(string userId, string partId, int privilege)
    {
        if (await partLogic.ChangePrivilege(Guid.Parse(userId), Guid.Parse(partId), privilege))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddMemberToPart([FromBody] PartMembersModel model)
    {
        if (await partLogic.AddToEntity(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveMemberFromPart([FromBody] PartMembersModel model)
    {
        if (await partLogic.RemoveFromEntity(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdatePart([FromBody] PartModel model)
    {
        if (await partLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }

    [HttpPost]
    [Route("link")]
    public async Task<IActionResult> LinkParts(string masterId, string slaveId)
    {
        if (await partLogic.LinkEntities(Guid.Parse(masterId), Guid.Parse(slaveId)))
            return Ok();
        return BadRequest();
    }
    
    [HttpPost]
    [Route("move")]
    public async Task<IActionResult> MovePart(string masterId, string slaveId)
    {
        if (await partLogic.UnlinkEntities(Guid.Parse(masterId), Guid.Parse(slaveId)))
            return Ok();
        return BadRequest();
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeletePart(string id)
    {
        if (await partLogic.DeleteEntity(Guid.Parse(id)))
            return Ok();
        return BadRequest();
    }
}