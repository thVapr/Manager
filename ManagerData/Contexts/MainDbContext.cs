using ManagerData.Constants;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ManagerData.Contexts;

public sealed class MainDbContext : DbContext
{
    public MainDbContext()
    {
        Database.Migrate();
    }

    public DbSet<MemberDataModel> Members { get; set; } = null!;
    public DbSet<PartDataModel> Parts { get; set; } = null!;
    public DbSet<TaskDataModel> Tasks { get; set; } = null!;
    public DbSet<PartMembersDataModel> PartMembers { get; set; } = null!;
    public DbSet<PartLink> PartLinks { get; set; } = null!;
    public DbSet<PartType> PartTypes { get; set; } = null!;
    public DbSet<PartTasksDataModel> PartTasks { get; set; } = null!;
    public DbSet<MemberTasksDataModel> MemberTasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartMembersDataModel>()
            .HasKey(k => new { k.PartId, k.MemberId });
        modelBuilder.Entity<PartMembersDataModel>()
            .HasOne(e => e.Part)
            .WithMany(e => e.PartMembers)
            .HasForeignKey(fk => fk.PartId);

        modelBuilder.Entity<PartLink>()
            .HasKey(k => new {k.MasterId, k.SlaveId});
        modelBuilder.Entity<PartLink>()
            .HasOne(e => e.MasterPart)
            .WithMany(e => e.SubParts)
            .HasForeignKey(fk => fk.MasterId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PartLink>()
            .HasOne(e => e.SlavePart)
            .WithOne(e => e.MainPart)
            .HasForeignKey<PartLink>(fk => fk.SlaveId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<PartTasksDataModel>()
            .HasKey(k => new {k.PartId, k.TaskId});
        modelBuilder.Entity<PartTasksDataModel>()
            .HasOne(e => e.Part)
            .WithMany(e => e.PartTasks)
            .HasForeignKey(fk => fk.PartId);

        modelBuilder.Entity<MemberTasksDataModel>()
            .HasKey(k => new { k.MemberId, k.TaskId });
        modelBuilder.Entity<MemberTasksDataModel>()
            .HasOne(e => e.Member)
            .WithMany(e => e.EmployeeTasks)
            .HasForeignKey(fk => fk.MemberId);
       
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var secretProvider = new SecretProvider();
        optionsBuilder.UseNpgsql(
            secretProvider.GetMainConnection()
        );
    }
}