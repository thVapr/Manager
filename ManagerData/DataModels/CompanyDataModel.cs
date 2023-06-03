
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerData.DataModels;

[Table("Companies")]
public class CompanyDataModel : BaseDataModel
{
    public Guid? ManagerId { get; set; }

    public IEnumerable<CompanyDepartmentsDataModel>? CompanyDepartments { get; set; }
}