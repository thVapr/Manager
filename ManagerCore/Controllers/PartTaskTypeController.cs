using ManagerLogic;
using ManagerCore.Models;
using ManagerCore.Utils;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts/types")]
[Authorize]
public class PartTaskTypeController(ITaskTypeLogic partTaskTypeLogic) : ControllerBase
{
        
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("get_all")]
    public async Task<IActionResult> GetTypes(string partId)
    {
        var types = await partTaskTypeLogic.GetPartTaskTypes(Guid.Parse(partId));
        if (types.Any())
            return Ok(types);
        return BadRequest($"У сущности с id {partId} нет доступных типов");
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddTaskTypeToPart([FromBody] AddToPartModel model)
    {
        if (await partTaskTypeLogic.AddTaskTypeToPart(Guid.Parse(model.PartId!), model.Name!))
            return Ok(true);
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveTaskTypeFromPart([FromBody] AddToPartModel model)
    {
        if (await partTaskTypeLogic.RemoveTaskTypeFromPart(Guid.Parse(model.PartId), Guid.Parse(model.EntityId!)))
            return Ok();
        return BadRequest();
    }
}