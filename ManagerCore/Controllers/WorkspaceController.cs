using ManagerLogic.Models;
using ManagerData.Constants;
using ManagerLogic.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/workspaces")]
[Authorize]
public class WorkspaceController : ControllerBase
{
    private readonly IManagementLogic<WorkspaceModel> _workspaceLogic;

    public WorkspaceController(IManagementLogic<WorkspaceModel> workspaceLogic)
    {
        _workspaceLogic = workspaceLogic;
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _workspaceLogic.GetEntities());
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await _workspaceLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> CreateModel(WorkspaceModel model)
    {
        if (await _workspaceLogic.CreateEntity(model))
            return Ok();
    
        return BadRequest();
    }
}