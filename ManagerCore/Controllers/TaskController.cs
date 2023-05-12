using ManagerCore.Models;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/task")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskLogic _taskLogic;

    public TaskController(ITaskLogic taskLogic)
    {
        _taskLogic = taskLogic;
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await _taskLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_free_tasks")]
    public async Task<IActionResult> GetFreeTasks(string id)
    {
        return Ok(await _taskLogic.GetFreeTasks(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_employee_tasks")]
    public async Task<IActionResult> GetEmployeeTasks(string id)
    {
        return Ok(await _taskLogic.GetEmployeeTasks(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetModels(string id)
    {
        return Ok(await _taskLogic.GetEntitiesById(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(TaskModel model)
    {
        if (await _taskLogic.CreateEntity(model))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddEmployeeToProject(EmployeesTasks model)
    {
        if (await _taskLogic.AddEmployeeToTask(Guid.Parse(model.EmployeeId), Guid.Parse(model.TaskId)))
            return Ok();

        return BadRequest();
    }
}