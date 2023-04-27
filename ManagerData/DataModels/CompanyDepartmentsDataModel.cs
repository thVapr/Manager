

namespace ManagerData.DataModels;

public class CompanyDepartmentsDataModel
{
    public Guid CompanyId { get; set; }
    public Guid DepartmentId { get; set; }

    public CompanyDataModel Company { get; set; } = null!;
    public DepartmentDataModel Department { get; set; } = null!;
}