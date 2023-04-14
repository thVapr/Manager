
using System.ComponentModel.DataAnnotations;

namespace ManagerData.DataModels;

public class BaseDataModel
{
    [Key]
    public Guid Id { get; set; }
}