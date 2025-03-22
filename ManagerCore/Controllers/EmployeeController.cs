using ManagerCore.ViewModels;
using ManagerLogic.Authentication;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/employee")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IMemberLogic _employeeLogic;
    private readonly IPartLogic _departmentLogic;
    private readonly IManagementLogic<WorkspaceModel> _companyLogic;
    private readonly IAuthentication _authentication;

    public EmployeeController(IMemberLogic employeeLogic,
                              IPartLogic departmentLogic,
                              IManagementLogic<WorkspaceModel> companyLogic,
                              IAuthentication authentication)
    {
        _employeeLogic = employeeLogic;
        _departmentLogic = departmentLogic;
        _companyLogic = companyLogic;
        _authentication = authentication;
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetEmployee(string id)
    {
        return Ok(await _employeeLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_employees")]
    public async Task<IActionResult> GetEmployeesWithoutProjects(string id)
    {
        var adminIds = await _authentication.GetAdminIds();
        var employees = await _employeeLogic.GetFreeMembersInPart(Guid.Parse(id));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_free_employees")]
    public async Task<IActionResult> GetEmployeesWithoutDepartment()
    {
        var adminIds = await _authentication.GetAdminIds();
        var employees = await _employeeLogic.GetMembersWithoutPart();

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string id)
    {
        var adminIds = await _authentication.GetAdminIds();
        var employees = await _employeeLogic.GetEntitiesById(Guid.Parse(id));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_part_members")]
    public async Task<IActionResult> GetAllEmployeesByProjectId(string id)
    {
        var adminIds = await _authentication.GetAdminIds();
        var employees = await _employeeLogic.GetMembersFromPart(Guid.Parse(id));

        return Ok(employees.Where(e => !adminIds.Contains(e.Id)));
    }

    [HttpGet]
    [Route("get_member_profile")]
    public async Task<IActionResult> GetEmployeeProfile(string id)
    {
        var employee = await _employeeLogic.GetEntityById(Guid.Parse(id));

        if (employee.Id != id) return BadRequest(); 

        //var employeeLinks = await _employeeLogic.GetEmployeeLinks(Guid.Parse(id));

        //var company = await _companyLogic.GetEntityById(employeeLinks.CompanyId);
        //var department = await _departmentLogic.GetEntityById(employeeLinks.DepartmentId);

        return Ok(new EmployeeViewModel()
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
    public async Task<IActionResult> CreateEmployee(EmployeeModel model)
    {
        if (await _employeeLogic.CreateEntity(model))
            return Ok();
        return BadRequest();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateEmployee(EmployeeModel model)
    {
        if (await _employeeLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }

}