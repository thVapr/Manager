
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Companies")]
public class CompanyDataModel : BaseDataModel
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;

    public IEnumerable<CompanyDepartmentsDataModel>? CompanyDepartments { get; set; }
}