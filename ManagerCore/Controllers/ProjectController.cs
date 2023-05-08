using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/project")]
public class ProjectController : ControllerBase
{
    private readonly IProjectLogic _projectLogic;

    public ProjectController(IProjectLogic projectLogic)
    {
        _projectLogic = projectLogic;
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await _projectLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(ProjectModel model)
    {
        if (await _projectLogic.CreateEntity(model))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddEmployeeToProject(string projectId, string employeeId)
    {
        if (await _projectLogic.AddEmployeeToProject(Guid.Parse(projectId), Guid.Parse(employeeId)))
            return Ok();

        return BadRequest();
    }
}