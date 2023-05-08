
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

    public DbSet<EmployeeDataModel> Employees { get; set; } = null!;
    public DbSet<ProjectDataModel> Projects { get; set; } = null!;
    public DbSet<TaskDataModel> Tasks { get; set; } = null!;
    public DbSet<CompanyDataModel> Companies { get; set; } = null!;
    public DbSet<DepartmentDataModel> Departments { get; set; } = null!;

    public DbSet<CompanyDepartmentsDataModel> CompanyDepartments { get; set; } = null!;
    public DbSet<DepartmentProjectsDataModel> DepartmentProjects { get; set; } = null!;
    public DbSet<EmployeeLinksDataModel> EmployeeLinks { get; set; } = null!;
    public DbSet<ProjectTasksDataModel> ProjectTasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyDepartmentsDataModel>()
            .HasKey(k => new { k.CompanyId, k.DepartmentId });

        modelBuilder.Entity<CompanyDepartmentsDataModel>()
            .HasOne(e => e.Company)
            .WithMany(e => e.CompanyDepartments)
            .HasForeignKey(fk => fk.CompanyId);

        modelBuilder.Entity<DepartmentProjectsDataModel>()
            .HasKey(k => new { k.DepartmentId, k.ProjectId });

        modelBuilder.Entity<DepartmentProjectsDataModel>()
            .HasOne(e => e.Department)
            .WithMany(e => e.DepartmentProjects)
            .HasForeignKey(fk => fk.DepartmentId);

        modelBuilder.Entity<EmployeeLinksDataModel>()
            .HasKey(k => new {k.EmployeeId, k.DepartmentId, k.ProjectId, k.TaskId});

        modelBuilder.Entity<EmployeeLinksDataModel>()
            .HasOne(e => e.Employee)
            .WithMany(e => e.EmployeeLink)
            .HasForeignKey(fk => fk.EmployeeId);

        modelBuilder.Entity<EmployeeLinksDataModel>()
            .HasOne(e => e.Department)
            .WithMany(e => e.EmployeeLinks)
            .HasForeignKey(fk => fk.DepartmentId);

        modelBuilder.Entity<EmployeeLinksDataModel>()
            .HasOne(e => e.Project)
            .WithMany(e => e.EmployeeLinks)
            .HasForeignKey(fk => fk.ProjectId);

        modelBuilder.Entity<ProjectTasksDataModel>()
            .HasKey(k => new {k.ProjectId, k.TaskId});

        modelBuilder.Entity<ProjectTasksDataModel>()
            .HasOne(e => e.Project)
            .WithMany(e => e.ProjectTasks);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            DataConstants.ManagerConnectionString
        );
    }
}