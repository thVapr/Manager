﻿namespace ManagerLogic.Models;

public class LoginModel
{
    public required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
}