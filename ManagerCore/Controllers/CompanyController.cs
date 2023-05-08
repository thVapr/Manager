using ManagerLogic.Management;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/company")]
public class CompanyController : ControllerBase
{
    private readonly IManagementLogic<CompanyModel> _companyLogic;

    public CompanyController(IManagementLogic<CompanyModel> companyLogic)
    {
        _companyLogic = companyLogic;
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetModel(string id)
    {
        return Ok(await _companyLogic.GetEntityById(Guid.Parse(id)));
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateModel(CompanyModel model)
    {
        if (await _companyLogic.CreateEntity(model))
            return Ok();
    
        return BadRequest();
    }
}