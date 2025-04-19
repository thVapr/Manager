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
    IMemberLogic employeeLogic,
    IPartLogic partLogic,
    IAuthentication authentication)
    : ControllerBase
{
    private readonly IPartLogic _partLogic = partLogic;

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetEmployee(string id)
    {
        return Ok(await employeeLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_members")]
    public async Task<IActionResult> GetEmployeesWithoutProjects(string id)
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await employeeLogic.GetFreeMembersInPart(Guid.Parse(id));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_free_members")]
    public async Task<IActionResult> GetEmployeesWithoutDepartment()
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await employeeLogic.GetMembersWithoutPart();

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string id)
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await employeeLogic.GetEntitiesById(Guid.Parse(id));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_part_members")]
    public async Task<IActionResult> GetAllEmployeesByProjectId(string id)
    {
        var adminIds = await authentication.GetAdminIds();
        var employees = await employeeLogic.GetMembersFromPart(Guid.Parse(id));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_member_profile")]
    public async Task<IActionResult> GetEmployeeProfile(string id)
    {
        var employee = await employeeLogic.GetEntityById(Guid.Parse(id));

        if (employee.Id != id) return BadRequest(); 

        // TODO: Добавить логику
        //var employeeLinks = await _employeeLogic.GetEmployeeLinks(Guid.Parse(id));

        //var company = await _companyLogic.GetEntityById(employeeLinks.CompanyId);
        //var department = await _departmentLogic.GetEntityById(employeeLinks.DepartmentId);

        return Ok(new MemberViewModel()
        //{
        //    Id = employee.Id,
            
        //    FirstName = employee.FirstName!,
        //    LastName = employee.LastName!,
        //    Patronymic = employee.Patronymic!,

        //    CompanyId = company.Id!,
        //    CompanyName = company.Name!,

        //    DepartmentId = department.Id!,
        //    DepartmentName = department.Name!,
        //}
        );

    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateEmployee(MemberModel model)
    {
        if (await employeeLogic.CreateEntity(model))
            return Ok();
        return BadRequest();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateEmployee(MemberModel model)
    {
        if (await employeeLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }

}