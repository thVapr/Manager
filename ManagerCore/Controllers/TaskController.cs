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
public class TaskController(ITaskLogic taskLogic, IFileLogic fileLogic) : ControllerBase
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
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [HttpGet]
    [Route("get_free_tasks")]
    public async Task<IActionResult> GetFreeTasks(string partId)
    {
        return Ok(await taskLogic.GetFreeTasks(Guid.Parse(partId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [HttpGet]
    [Route("get_available_tasks")]
    public async Task<IActionResult> GetAvailableTasks(string memberId, string partId)
    {
        return Ok(await taskLogic.GetAvailableTasks(Guid.Parse(memberId), Guid.Parse(partId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpGet]
    [Route("get_available_members_for_task")]
    public async Task<IActionResult> GetAvailableMembersForTask(string partId, string taskId)
    {
        return Ok(await taskLogic.GetAvailableMembersForTask( Guid.Parse(partId), Guid.Parse(taskId)));
    }

    [HttpGet]
    [Route("get_member_tasks")]
    public async Task<IActionResult> GetMemberTasks(string memberId)
    {
        return Ok(await taskLogic.GetMemberTasks(Guid.Parse(memberId)));
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpGet]
    [Route("get_task_history")]
    public async Task<IActionResult> GetTaskHistory(string taskId)
    {
        return Ok(await taskLogic.GetTaskHistory(Guid.Parse(taskId)));
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetModels(string partId)
    {
        return Ok((await taskLogic.GetEntitiesById(Guid.Parse(partId))));
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
    [Route("add_member")]
    public async Task<IActionResult> AddMemberToTask([FromBody] MemberTasks model)
    {
        var isMemberAdded = await taskLogic.AddMemberToTask(
            Guid.Parse(User.FindFirst("id")?.Value!),
            Guid.Parse(model.MemberId!), 
            Guid.Parse(model.TaskId!),
            model.GroupId!.Value);
        if (isMemberAdded)
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpPost]
    [Route("remove_member")]
    public async Task<IActionResult> RemoveMemberToTask([FromBody] MemberTasks model)
    {
        var isMemberRemoved = await taskLogic.RemoveMemberFromTask(
            Guid.Parse(User.FindFirst("id")?.Value!),
            Guid.Parse(model.MemberId!), 
            Guid.Parse(model.TaskId!));
        if (isMemberRemoved)
            return Ok();
        return BadRequest();
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpPatch]
    [Route("change")]
    public async Task<IActionResult> ChangeTaskStatus([FromBody] ChangeTaskStatusModel model)
    {
        var task = await taskLogic.GetEntityById(Guid.Parse(model.PartId));
        if (task.PartId != null && task.PartId == Guid.Parse(model.PartId))
            return Forbid();
        return Ok(
            await taskLogic.ChangeTaskStatus(
                new HistoryModel
                {
                    InitiatorId = User.FindFirst("id")?.Value!,
                    Description = model.Description ?? string.Empty,
                    Name = model.Name ?? string.Empty,
                },
                Guid.Parse(model.TaskId!),
                model.Forward)
        );
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateTask(
        UpdateTaskRequest model)
    {
        var history = new HistoryModel
        {
            InitiatorId = User.FindFirst("id")?.Value!,
            Description = model.Description ?? string.Empty,
            Name = model.Name ?? string.Empty,
        };
        
        var isTaskUpdated = await taskLogic.UpdateTask(
            history!,
            model.Task
        );
        
        if (isTaskUpdated)
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpPost]
    [Route("upload_file")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string taskId)
    {
        await fileLogic.Upload(file, taskId);
        return Ok();
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpGet]
    [Route("get_files")]
    public async Task<IActionResult> GetFileList(string taskId)
    {
        var list = await fileLogic.GetFileList(taskId);
        return Ok(list);
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpGet]
    [Route("download_file")]
    public async Task<IActionResult> GetFile(string fileName, string taskId)
    {
        var stream = await fileLogic.Download(fileName, taskId);
        string contentType = "application/octet-stream";
        stream.Position = 0;
        return File(stream, contentType, fileName);
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Create])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeletePart(string partId, string taskId)
    {
        var task = await taskLogic.GetEntityById(Guid.Parse(taskId));
        if (task.PartId != null && task.PartId != Guid.Parse(partId))
            Forbid($"У вас нет прав для доступа к задаче с id: {taskId}");
        if (await taskLogic.DeleteEntity(Guid.Parse(taskId)))
            return Ok();
        return BadRequest();
    }
}