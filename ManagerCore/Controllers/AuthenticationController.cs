using ManagerLogic.Models;
using ManagerData.Contexts;
using Microsoft.AspNetCore.Mvc;
using ManagerData.Authentication;
using ManagerLogic.Authentication;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthentication _authentication;

    public AuthenticationController(IAuthentication authentication)
    {
        _authentication = authentication;
    }

    //TODO: delete this method
    [HttpGet]
    public async Task<IActionResult> GetUser(string email)
    {
        var context = new AuthenticationDbContext();
        var data = new AuthenticationRepository(context);

        var user = await data.GetUser(email);
                    
        return Ok(user);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(LoginModel user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        await _authentication.CreateUser(user);
        return await Login(user);
    }
                
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(await _authentication.Authenticate(user));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(string token)
    {
        var qToken = await _authentication.Logout(token);

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshModel? model)
    {
        if (model is null)
            return BadRequest();

        return Ok(await _authentication.UpdateToken(model));

    }
}
