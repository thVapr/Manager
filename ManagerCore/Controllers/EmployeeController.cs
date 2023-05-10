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

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddEmployeeToDepartment(string departmentId, string employeeId)
    {
        if (await _employeeLogic.AddEmployeeToDepartment(Guid.Parse(departmentId), Guid.Parse(employeeId)))
            return Ok();

        return BadRequest();
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