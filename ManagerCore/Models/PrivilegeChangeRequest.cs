using ManagerLogic.Models;
using System.ComponentModel.DataAnnotations;

namespace ManagerCore.Models;

public class PrivilegeChangeRequest : IPartAllocationModel
{
    public int Privilege { get; set; }
}