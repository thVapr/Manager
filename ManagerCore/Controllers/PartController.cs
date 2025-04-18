using ManagerCore.Models;
using ManagerLogic.Models;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts")]
[Authorize]
public class PartController : ControllerBase 
{
    private readonly IPartLogic _partLogic;

    public PartController(IPartLogic partLogic)
    {
        _partLogic = partLogic;
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string id)
    {
        return Ok(await _partLogic.GetEntitiesById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await _partLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(PartModel model)
    {
        if (await _partLogic.CreateEntity(model))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddMemberToPart([FromBody] PartMembersModel model)
    {
        if (await _partLogic.AddEmployeeToDepartment(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveMemberFromPart([FromBody] PartMembersModel model)
    {
        if (await _partLogic.RemoveEmployeeFromDepartment(Guid.Parse(model.PartId!), Guid.Parse(model.MemberId!)))
            return Ok();

        return BadRequest();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdatePart([FromBody] PartModel model)
    {
        if (await _partLogic.UpdateEntity(model))
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