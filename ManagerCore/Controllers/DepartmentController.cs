using ManagerCore.Models;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/department")]
[Authorize]
public class DepartmentController : ControllerBase 
{
    private readonly IDepartmentLogic _departmentLogic;

    public DepartmentController(IDepartmentLogic departmentLogic)
    {
        _departmentLogic = departmentLogic;
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll(string id)
    {
        return Ok(await _departmentLogic.GetEntitiesById(Guid.Parse(id)));
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await _departmentLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(DepartmentModel model)
    {
        if (await _departmentLogic.CreateEntity(model))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddEmployeeToDepartment([FromBody] DepartmentEmployeesModel model)
    {
        if (await _departmentLogic.AddEmployeeToDepartment(Guid.Parse(model.DepartmentId), Guid.Parse(model.EmployeeId)))
            return Ok();

        return BadRequest();
    }

    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveEmployeeFromDepartment([FromBody] DepartmentEmployeesModel model)
    {
        if (await _departmentLogic.RemoveEmployeeFromDepartment(Guid.Parse(model.DepartmentId), Guid.Parse(model.EmployeeId)))
            return Ok();

        return BadRequest();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentModel model)
    {
        if (await _departmentLogic.UpdateEntity(model))
            return Ok();
        return BadRequest();
    }
}