using ManagerData.Authentication;
using ManagerData.Contexts;
using ManagerLogic.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/test")]
public class TestController : ControllerBase
{
    private readonly IAuthentication _authentication;

    public TestController(IAuthentication authentication)
    {
        _authentication = authentication;
    }

/*    [HttpGet]
    [Route("data")]
    public async Task<IActionResult> GetUser(string email = "vaprmail@gmail.com")
    {
        return Ok();
    }*/
}