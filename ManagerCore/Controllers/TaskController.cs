using ManagerLogic;
using ManagerCore.Utils;
using ManagerCore.Models;
using ManagerLogic.Models;
using ManagerLogic.Management;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/parts/{partId}/tasks")]
[Authorize]
public class TaskController(
    ITaskLogic taskLogic, IFileLogic fileLogic, ITaskMessageLogic taskMessageLogic) : ControllerBase
{
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    public async Task<IActionResult> GetTask(string taskId, string partId)
    {
        return Ok(await taskLogic.GetById(Guid.Parse(taskId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetModels(string partId)
    {
        return Ok((await taskLogic.GetManyById(Guid.Parse(partId))));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [HttpGet]
    [Route("free")]
    public async Task<IActionResult> GetFreeTasks(string partId)
    {
        return Ok(await taskLogic.GetFreeTasks(Guid.Parse(partId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [HttpGet]
    [Route("available")]
    public async Task<IActionResult> GetAvailableTasks(string memberId, string partId)
    {
        return Ok(await taskLogic.GetAvailableTasks(Guid.Parse(memberId), Guid.Parse(partId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> SearchTasks(string query, string partId)
    {
        return Ok(await taskLogic.GetByQuery(query, Guid.Parse(partId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Create])]
    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskModel model, string partId)
    {
        if (await taskLogic.Create(model))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpPatch]
    [Route("{taskId}")]
    public async Task<IActionResult> ChangeTaskStatus([FromBody] ChangeTaskStatusModel model, string taskId, string partId)
    {
        var task = await taskLogic.GetById(Guid.Parse(partId));
        if (task.PartId != null && task.PartId == Guid.Parse(partId))
            return Forbid();
        return Ok(
            await taskLogic.ChangeTaskStatus(
                new HistoryModel
                {
                    InitiatorId = User.FindFirst("id")?.Value!,
                    Description = model.Description ?? string.Empty,
                    Name = model.Name ?? string.Empty,
                },
                Guid.Parse(taskId!),
                model.Forward)
        );
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpPut]
    [Route("{taskId}")]
    public async Task<IActionResult> UpdateTask(
        UpdateTaskRequest model, string taskId, string partId)
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
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpDelete]
    [Route("{taskId}")]
    public async Task<IActionResult> DeleteTask(string partId, string taskId)
    {
        var task = await taskLogic.GetById(Guid.Parse(taskId));
        if (task.PartId != null && task.PartId != Guid.Parse(partId))
            Forbid($"У вас нет прав для доступа к задаче с id: {taskId}");
        if (await taskLogic.Delete(Guid.Parse(taskId)))
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpGet]
    [Route("history")]
    public async Task<IActionResult> GetTaskHistory(string taskId, string partId)
    {
        return Ok(await taskLogic.GetTaskHistory(Guid.Parse(taskId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Read])]
    [HttpGet]
    [Route("{taskId}/members")]
    public async Task<IActionResult> GetTaskMembers(string taskId, string partId)
    {
        return Ok(await taskLogic.GetTaskMembers(Guid.Parse(taskId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [HttpGet]
    [Route("{taskId}/members/available")]
    public async Task<IActionResult> GetAvailableMembersForTask(string taskId, string partId)
    {
        return Ok(await taskLogic.GetAvailableMembersForTask( Guid.Parse(partId), Guid.Parse(taskId)));
    }

    [HttpGet]
    [Route("members/assigned")]
    public async Task<IActionResult> GetMemberTasks(string memberId, string partId)
    {
        return Ok(await taskLogic.GetMemberTasks(Guid.Parse(memberId)));
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpPost]
    [Route("{taskId}/members")]
    public async Task<IActionResult> AddMemberToTask([FromBody] MemberTasks model, string taskId, string partId)
    {
        var isMemberAdded = await taskLogic.AddMemberToTask(
            Guid.Parse(User.FindFirst("id")?.Value!),
            Guid.Parse(model.MemberId!), 
            Guid.Parse(taskId),
            model.GroupId!.Value);
        if (isMemberAdded)
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpDelete]
    [Route("{taskId}/members")]
    public async Task<IActionResult> RemoveMemberFromTask(string memberId, string taskId, string partId)
    {
        var isMemberRemoved = await taskLogic.RemoveMemberFromTask(
            Guid.Parse(User.FindFirst("id")?.Value!),
            Guid.Parse(memberId!), 
            Guid.Parse(taskId!));
        if (isMemberRemoved)
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpPost]
    [Route("{taskId}/files")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request, [FromRoute] string taskId, [FromRoute] string partId)
    {
        var history = new HistoryModel
        {
            InitiatorId = User.FindFirst("id")?.Value!,
            PartId = partId
        };
        
        await fileLogic.Upload(history, request.File, taskId);
        return Ok();
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpGet]
    [Route("{taskId}/files")]
    public async Task<IActionResult> GetFileList(string taskId, string partId)
    {
        var list = await fileLogic.GetFileList(taskId);
        return Ok(list);
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpGet]
    [Route("{taskId}/files/{fileName}")]
    public async Task<IActionResult> GetFile(string fileName, string taskId, string partId)
    {
        var stream = await fileLogic.Download(fileName, taskId);
        string contentType = "application/octet-stream";
        stream.Position = 0;
        
        return File(stream, contentType, fileName);
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpDelete]
    [Route("{taskId}/files/{fileName}")]
    public async Task<IActionResult> DeleteFile(string fileName, string taskId, string partId)
    {
        var history = new HistoryModel
        {
            InitiatorId = User.FindFirst("id")?.Value!,
            PartId = partId
        };
        
        await fileLogic.Remove(history, fileName, taskId);
        return Ok();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Take])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpGet]
    [Route("{taskId}/messages")]
    public async Task<IActionResult> GetMessagesFromTask(string partId, string taskId)
    {
        return Ok(await taskMessageLogic.GetTaskMessages(Guid.Parse(taskId)));
    }
    
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpPost]
    [Route("{taskId}/messages")]
    public async Task<IActionResult> AddMessageToTask([FromBody] TaskMessageModel model, string taskId, string partId)
    {
        model.PartId = partId;
        var isMessageAdded = await taskMessageLogic.CreateAsync(model);
        if (isMessageAdded)
            return Ok();
        return BadRequest();
    }
    
    [TypeFilter(typeof(PartAccessFilter), Arguments = [(int)AccessLevel.Control])]
    [TypeFilter(typeof(TaskAccessFilter))]
    [HttpDelete]
    [Route("{taskId}/messages")]
    public async Task<IActionResult> RemoveMessageFromTask(string partId, string taskId, string messageId)
    {
        var isMessageRemoved = await taskMessageLogic.DeleteAsync(Guid.Parse(messageId));
        if (isMessageRemoved)
            return Ok();
        return BadRequest();
    }
}