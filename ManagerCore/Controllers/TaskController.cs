using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/task")]
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
    public async Task<IActionResult> AddEmployeeToProject(string employeeId, string taskId)
    {
        if (await _taskLogic.AddEmployeeToTask(Guid.Parse(employeeId), Guid.Parse(taskId)))
            return Ok();

        return BadRequest();
    }
}