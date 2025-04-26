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
    public async Task<IActionResult> GetMember(string id)
    {
        return Ok(await memberLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_members")]
    public async Task<IActionResult> GetMembersWithoutPart(string id)
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await memberLogic.GetFreeMembersInPart(Guid.Parse(id));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_free_members")]
    public async Task<IActionResult> GetMembersWithoutAnyPart()
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await memberLogic.GetMembersWithoutPart();

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string id)
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await memberLogic.GetEntitiesById(Guid.Parse(id));

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