using ManagerCore.Utils;
using ManagerCore.ViewModels;
using ManagerLogic.Authentication;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/members")]
[Authorize]
public class MemberController(
    IMemberLogic memberLogic,
    IPartLogic partLogic,
    IAuthentication authentication)
    : ControllerBase
{
    private readonly IPartLogic _partLogic = partLogic;

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetMember(string memberId)
    {
        return Ok(await memberLogic.GetEntityById(Guid.Parse(memberId)));
    }

    [TypeFilter(typeof(PartAccessFilter), Arguments = [5])]
    [HttpGet]
    [Route("get_members")]
    public async Task<IActionResult> GetAvailableMembersPart(string? partId)
    {
        var adminIds = await authentication.GetAdminIds();
        var members = await memberLogic.GetAvailableMembers(Guid.Parse(partId!));
        var part = await _partLogic.GetEntityById(Guid.Parse(partId!));
        
        if (part.Level == 0)
        {
            var availableUsersIds = await authentication.GetAvailableUserIds();
            var membersWithoutPart = await memberLogic.GetMembersWithoutPart();
            members = members.Union(membersWithoutPart.Where(m => availableUsersIds.Contains(m.Id!))).ToList();
        }
        
        if (members.Count != 0)
            return Ok(members.Where(e => !adminIds.Contains(e.Id)));
        return NotFound("There are no users satisfying the request");
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string partId)
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await memberLogic.GetEntitiesById(Guid.Parse(partId));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_part_members")]
    public async Task<IActionResult> GetAllMembersByPartId(string id)
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await memberLogic.GetMembersFromPart(Guid.Parse(id));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_member_profile")]
    public async Task<IActionResult> GetMemberProfile(string id)
    {
        var member = await memberLogic.GetEntityById(Guid.Parse(id));

        if (member.Id != id) return BadRequest(); 

        // TODO: Добавить логику
        //var employeeLinks = await _employeeLogic.GetEmployeeLinks(Guid.Parse(id));

        //var company = await _companyLogic.GetEntityById(employeeLinks.CompanyId);
        //var department = await _departmentLogic.GetEntityById(employeeLinks.DepartmentId);

        return Ok(new MemberViewModel()
        {
            Id = member.Id,
          
            FirstName = member.FirstName!,
            LastName = member.LastName!,
            Patronymic = member.Patronymic!,
        }
        );

    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateMember(MemberModel model)
    {
        if (await memberLogic.CreateEntity(model))
            return Ok();
        return BadRequest();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateMember(MemberModel model)
    {
        if (await memberLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }

}