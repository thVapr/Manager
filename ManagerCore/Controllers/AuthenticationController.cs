﻿using ManagerLogic.Models;
using Microsoft.AspNetCore.Mvc;
using ManagerLogic.Authentication;

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
        
        return BadRequest("Пользователь с таким именем уже существует");
    }
                
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Некорректный логин или пароль");
        }

        var tokenPair = await authentication.Authenticate(user);
        return !string.IsNullOrWhiteSpace(tokenPair.Item1) 
            ? Ok(tokenPair) 
            : StatusCode(401, "Неподходящая пара логина и пароля");
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
            return BadRequest("Токен не валиден");

        return Ok(await authentication.UpdateToken(model));
    }
}
