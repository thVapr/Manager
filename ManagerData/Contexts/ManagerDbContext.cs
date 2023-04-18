
using ManagerData.Constants;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Contexts;

public sealed class ManagerDbContext : DbContext
{
    public ManagerDbContext()
    {
        Database.EnsureCreated();
    }

    public DbSet<EmployeeDataModel> Users { get; set; } = null!;
    public DbSet<ProjectDataModel> Projects { get; set; } = null!;
    public DbSet<TaskDataModel> Tasks { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            DataConstants.ManagerConnectionString
        );
    }
}