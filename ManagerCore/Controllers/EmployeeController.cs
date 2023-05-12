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
    private readonly IEmployeeLogic _employeeLogic;

    public EmployeeController(IEmployeeLogic employeeLogic)
    {
        _employeeLogic = employeeLogic;
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
        return Ok(await _employeeLogic.GetEmployeesWithoutProjectByDepartmentId(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_free_employees")]
    public async Task<IActionResult> GetEmployeesWithoutProjects()
    {
        return Ok(await _employeeLogic.GetEmployeesWithoutDepartment());
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string id)
    {
        return Ok(await _employeeLogic.GetEntitiesById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get_project_employees")]
    public async Task<IActionResult> GetAllEmployeesByProjectId(string id)
    {
        return Ok(await _employeeLogic.GetEmployeesFromProject(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateEmployee(EmployeeModel model)
    {
        if (await _employeeLogic.CreateEntity(model))
            return Ok();

        return BadRequest();
    }
}