using ManagerLogic;
using ManagerCore.Models;
using ManagerCore.Utils;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts/{partId}/types")]
[Authorize]
public class PartTaskTypeController(ITaskTypeLogic partTaskTypeLogic) : ControllerBase
{
        
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetTypes(string partId)
    {
        var types = await partTaskTypeLogic.GetPartTaskTypes(Guid.Parse(partId));
        if (types.Any())
            return Ok(types);
        return BadRequest($"У сущности с id {partId} нет доступных типов");
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> AddTaskTypeToPart(string partId, [FromBody] AddToPartModel model)
    {
        if (await partTaskTypeLogic.AddTaskTypeToPart(Guid.Parse(partId), model.Name!))
            return Ok(true);
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpDelete]
    [Route("{entityId}")]
    public async Task<IActionResult> RemoveTaskTypeFromPart(string partId, string entityId)
    {
        if (await partTaskTypeLogic.RemoveTaskTypeFromPart(Guid.Parse(partId), Guid.Parse(entityId!)))
            return Ok();
        return BadRequest();
    }
}