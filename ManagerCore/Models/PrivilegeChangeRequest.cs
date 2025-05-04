using System.ComponentModel.DataAnnotations;

namespace ManagerCore.Models;

public class PrivilegeChangeRequest
{
    [Required(ErrorMessage = "MemberId is required")]
    public string MemberId { get; set; }
    [Required(ErrorMessage = "PartId is required")]
    public string PartId { get; set; }
    [Required(ErrorMessage = "Privilege is required")]
    public int Privilege { get; set; }
}