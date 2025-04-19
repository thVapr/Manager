using ManagerCore.Models;
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
    [Route("all")]
    public async Task<IActionResult> GetAll(string id)
    {
        return Ok(await partLogic.GetEntitiesById(Guid.Parse(id)));
    }

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
    [Route("add")]
    public async Task<IActionResult> AddMemberToPart([FromBody] PartMembersModel model)
    {
        if (await partLogic.AddEmployeeToDepartment(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveMemberFromPart([FromBody] PartMembersModel model)
    {
        if (await partLogic.RemoveEmployeeFromDepartment(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
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
        return StatusCode(501);
    }
    
    [HttpPost]
    [Route("move")]
    public async Task<IActionResult> MovePart(string slaveId)
    {
        return StatusCode(501);
    }
}