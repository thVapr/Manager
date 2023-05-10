using ManagerData.Constants;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Mvc;
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
