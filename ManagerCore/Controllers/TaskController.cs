using ManagerCore.Models;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/tasks")]

public class TaskController(ITaskLogic taskLogic) : ControllerBase
{
    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await taskLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_free_tasks")]
    public async Task<IActionResult> GetFreeTasks(string id)
    {
        return Ok(await taskLogic.GetFreeTasks(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_member_tasks")]
    public async Task<IActionResult> GetEmployeeTasks(string id)
    {
        return Ok(await taskLogic.GetMemberTasks(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetModels(string id)
    {

        return Ok((await taskLogic.GetEntitiesById(Guid.Parse(id)))
            .Select(t => new TaskModel()
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                CreatorId = t.CreatorId,
                EmployeeId = t.EmployeeId,
                Level = t.Level,
                Status = t.Status,
            }));
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> SearchTasks(string query, string id)
    {
        return Ok(await taskLogic.GetEntitiesByQuery(query, Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(TaskModel model)
    {
        if (await taskLogic.CreateEntity(model))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddEmployeeToProject(MemberTasks model)
    {
        if (await taskLogic.AddMemberToTask(Guid.Parse(model.MemberId!), Guid.Parse(model.TaskId!)))
            return Ok();

        return BadRequest();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateTask(TaskModel model)
    {
        if (await taskLogic.UpdateEntity(model))
            return Ok();

        return BadRequest();
    }

}