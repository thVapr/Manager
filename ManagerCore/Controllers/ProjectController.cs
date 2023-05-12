using ManagerCore.Models;
using ManagerData.DataModels;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/project")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectLogic _projectLogic;

    public ProjectController(IProjectLogic projectLogic)
    {
        _projectLogic = projectLogic;
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetModels(string id)
    {
        return Ok(await _projectLogic.GetEntitiesById(Guid.Parse(id)));
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
    public async Task<IActionResult> AddEmployeeToProject(ProjectEmployeesModel model)
    {
        if (await _projectLogic.AddEmployeeToProject(Guid.Parse(model.ProjectId), Guid.Parse(model.EmployeeId)))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveEmployeeFromProject(ProjectEmployeesModel model)
    {
        if (await _projectLogic.RemoveEmployeeFromProject(Guid.Parse(model.ProjectId), Guid.Parse(model.EmployeeId)))
            return Ok();

        return BadRequest();
    }
}