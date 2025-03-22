using ManagerData.Constants;
using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/company")]
[Authorize]
public class CompanyController : ControllerBase
{
    private readonly IManagementLogic<WorkspaceModel> _companyLogic;

    public CompanyController(IManagementLogic<WorkspaceModel> companyLogic)
    {
        _companyLogic = companyLogic;
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _companyLogic.GetEntities());
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await _companyLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> CreateModel(WorkspaceModel model)
    {
        if (await _companyLogic.CreateEntity(model))
            return Ok();
    
        return BadRequest();
    }
}