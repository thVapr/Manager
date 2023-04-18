using ManagerLogic.Models;
using ManagerData.Contexts;
using Microsoft.AspNetCore.Mvc;
using ManagerData.Authentication;
using ManagerLogic.Authentication;
using Microsoft.AspNetCore.Authorization;

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
    //[Authorize]
    [HttpGet]
    [Route("data")]
    public async Task<IActionResult> GetUser(string email = "vaprmail@gmail.com")
    {
        var secontext = new ManagerDbContext();
        var context = new AuthenticationDbContext();
        var data = new AuthenticationRepository(context);

        var user = await data.GetUser(email);
        
        return Ok(new {email = user.Email});
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(LoginModel user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (await _authentication.CreateUser(user))
        {
            return await Login(user);
        }
        
        return BadRequest();
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

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenModel model)
    {
        return await _authentication.Logout(model.RefreshToken) ? Ok() : BadRequest();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshModel? model)
    {
        if (model is null)
            return BadRequest();

        return Ok(await _authentication.UpdateToken(model));

    }
}
