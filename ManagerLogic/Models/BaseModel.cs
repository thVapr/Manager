﻿
namespace ManagerLogic.Models;

public class BaseModel
{
    public string? Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}