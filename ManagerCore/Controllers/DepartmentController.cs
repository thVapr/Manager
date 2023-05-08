using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/department")]
public class DepartmentController : ControllerBase 
{
    private readonly IManagementLogic<DepartmentModel> _departmentLogic;

    public DepartmentController(IManagementLogic<DepartmentModel> depatmentLogic)
    {
        _departmentLogic = depatmentLogic;
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
}