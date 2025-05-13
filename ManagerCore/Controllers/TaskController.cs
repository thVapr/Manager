using ManagerLogic;
using ManagerCore.Utils;
using ManagerCore.Models;
using ManagerLogic.Models;
using ManagerLogic.Management;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/tasks")]
[Authorize]
public class TaskController(ITaskLogic taskLogic, IPartLogic partLogic) : ControllerBase
{
    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string taskId)
    {
        return Ok(await taskLogic.GetEntityById(Guid.Parse(taskId)));
    }

    [HttpGet]
    [Route("get_task_members")]
    public async Task<IActionResult> GetTaskMembers(string taskId)
    {
        return Ok(await taskLogic.GetTaskMembers(Guid.Parse(taskId)));
    }
    
    [HttpGet]
    [Route("get_free_tasks")]
    public async Task<IActionResult> GetFreeTasks(string partId)
    {
        return Ok(await taskLogic.GetFreeTasks(Guid.Parse(partId)));
    }

    [HttpGet]
    [Route("get_member_tasks")]
    public async Task<IActionResult> GetMemberTasks(string memberId)
    {
        return Ok(await taskLogic.GetMemberTasks(Guid.Parse(memberId)));
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetModels(string partId)
    {

        return Ok((await taskLogic.GetEntitiesById(Guid.Parse(partId)))
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

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> SearchTasks(string query, string partId)
    {
        return Ok(await taskLogic.GetEntitiesByQuery(query, Guid.Parse(partId)));
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Create])]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(TaskModel model)
    {
        if (await taskLogic.CreateEntity(model))
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddMemberToTask([FromBody] MemberTasks model)
    {
        if (await taskLogic.AddMemberToTask(Guid.Parse(model.MemberId!), Guid.Parse(model.TaskId!), model.GroupId!.Value))
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [HttpPatch]
    [Route("change")]
    public async Task<IActionResult> ChangeTaskStatus(string partId, string taskId, string? name, string? description)
    {
        var task = await taskLogic.GetEntityById(Guid.Parse(partId));
        if (task.PartId != null && task.PartId == Guid.Parse(partId))
            return Forbid();
        return Ok(
            await taskLogic.ChangeTaskStatus(
                new HistoryModel
                {
                    InitiatorId = User.FindFirst("id")?.Value!,
                    Description = description ?? string.Empty,
                    Name = name ?? string.Empty,
                },
                Guid.Parse(taskId!))
        );
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateTask(TaskModel model)
    {
        if (await taskLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }
}