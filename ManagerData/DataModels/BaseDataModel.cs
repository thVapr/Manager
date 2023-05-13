
using System.ComponentModel.DataAnnotations;

namespace ManagerData.DataModels;

public abstract class BaseDataModel
{
    [Key]
    public Guid Id { get; init; }
}