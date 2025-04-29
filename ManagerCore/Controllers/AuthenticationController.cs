using ManagerLogic.Authentication;
using ManagerLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerCore.Controllers;

[ApiController]
[Route("/api/authentication")]
public class AuthenticationController(IAuthentication authentication) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(LoginModel user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (await authentication.CreateUser(user))
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

        var tokenPair = await authentication.Authenticate(user);
        return !string.IsNullOrWhiteSpace(tokenPair.Item1) ? 
            Ok(tokenPair) : StatusCode(401, "Invalid username or password");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenModel model)
    {
        return await authentication.Logout(model.RefreshToken!) ? Ok() : BadRequest();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshModel? model)
    {
        if (model is null)
            return BadRequest();

        return Ok(await authentication.UpdateToken(model));

    }
}
