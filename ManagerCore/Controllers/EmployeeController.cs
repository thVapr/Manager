using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/employee")]
public class EmployeeController : ControllerBase
{
    private readonly IManagementLogic<EmployeeModel> _employeeLogic;

    public EmployeeController(IManagementLogic<EmployeeModel> employeeLogic)
    {
        _employeeLogic = employeeLogic;
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await _employeeLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(EmployeeModel model)
    {
        if (await _employeeLogic.CreateEntity(model))
            return Ok();

        return BadRequest();
    }
}