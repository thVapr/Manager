using ManagerCore.Models;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/tasks")]
[Authorize]
public class TaskController(ITaskLogic taskLogic) : ControllerBase
{
    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await taskLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_task_members")]
    public async Task<IActionResult> GetTaskMembers(string taskId)
    {
        return Ok(await taskLogic.GetTaskMembers(Guid.Parse(taskId)));
    }
    
    [HttpGet]
    [Route("get_free_tasks")]
    public async Task<IActionResult> GetFreeTasks(string id)
    {
        return Ok(await taskLogic.GetFreeTasks(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_member_tasks")]
    public async Task<IActionResult> GetMemberTasks(string id)
    {
        return Ok(await taskLogic.GetMemberTasks(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetModels(string id)
    {

        return Ok((await taskLogic.GetEntitiesById(Guid.Parse(id)))
            .Select(task => new TaskModel()
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                CreatorId = task.CreatorId,
                Deadline = task.Deadline,
                StartTime = task.StartTime,
                ClosedAt = task.ClosedAt,
                Path = task.Path,
                Level = task.Level,
                Status = task.Status,
                Priority = task.Priority,
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
    public async Task<IActionResult> AddMemberToTask([FromBody] MemberTasks model)
    {
        if (await taskLogic.AddMemberToTask(Guid.Parse(model.MemberId!), Guid.Parse(model.TaskId!), model.GroupId!.Value))
            return Ok();
        return BadRequest();
    }

    [HttpPatch]
    [Route("change")]
    public async Task<IActionResult> ChangeTaskStatus(string taskId)
    {
        return Ok(await taskLogic.ChangeTaskStatus(Guid.Parse(taskId!)));
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