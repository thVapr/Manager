using ManagerCore.Models;
using ManagerCore.Utils;
using ManagerData.Constants;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts")]
[Authorize]
public class PartController(IPartLogic partLogic) : ControllerBase
{
    [HttpGet]
    [Route("all_accessible")]
    public async Task<IActionResult> GetAll()
    {
        if (User.IsInRole(RoleConstants.Admin))
            return Ok(await partLogic.GetEntities());
        var userId = User.FindFirst("id")?.Value;
        return Ok(await partLogic.GetAllAccessibleParts(Guid.Parse(userId!)));
    }
    
    [HttpPost]
    [Route("update_hierarchy")]
    public async Task<IActionResult> UpdateHierarchy(List<PartModel> models)
    {
        return Ok(await partLogic.UpdateHierarchy(models));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [1])]
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
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [3])]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(PartModel model)
    {
        var userId = User.FindFirst("id")?.Value;
        if (await partLogic.CreatePart(Guid.Parse(userId!),model))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [5])]
    [HttpPost]
    [Route("change_privilege")]
    public async Task<IActionResult> ChangePrivilege(string userId, string partId, int privilege)
    {
        if (await partLogic.ChangePrivilege(Guid.Parse(userId), Guid.Parse(partId), privilege))
            return Ok();

        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [5])]
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddMemberToPart([FromBody] PartMembersModel model)
    {
        if (await partLogic.AddToEntity(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [5])]
    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveMemberFromPart([FromBody] PartMembersModel model)
    {
        if (await partLogic.RemoveFromEntity(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [5])]
    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdatePart([FromBody] PartModel model)
    {
        if (await partLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [5])]
    [HttpPost]
    [Route("link")]
    public async Task<IActionResult> LinkParts(string masterId, string slaveId)
    {
        if (await partLogic.LinkEntities(Guid.Parse(masterId), Guid.Parse(slaveId)))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [5])]
    [HttpPost]
    [Route("move")]
    public async Task<IActionResult> MovePart(string masterId, string slaveId)
    {
        if (await partLogic.UnlinkEntities(Guid.Parse(masterId), Guid.Parse(slaveId)))
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [5])]
    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeletePart(string id)
    {
        if (await partLogic.DeleteEntity(Guid.Parse(id)))
            return Ok();
        return BadRequest();
    }
}