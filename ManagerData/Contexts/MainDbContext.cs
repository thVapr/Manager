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
    public DbSet<PartMemberDataModel> PartMembers { get; set; } = null!;
    public DbSet<TaskMember> TaskMembers { get; set; } = null!;
    public DbSet<PartType> PartTypes { get; set; } = null!;
    public DbSet<PartTaskStatus> PartTaskStatuses { get; set; } = null!;
    public DbSet<TaskHistory> TaskHistories { get; set; } = null!;
    public DbSet<PartRole> PartRoles { get; set; } = null!;
    public DbSet<BackgroundTask> BackgroundTasks { get; set; } = null!;
    public DbSet<PartMemberRole> PartMemberRoles { get; set; } = null!;
    public DbSet<TagDataModel> Tags { get; set; } = null!;
    public DbSet<MemberTag> MemberTags { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartDataModel>()
            .HasMany(fk => fk.Members)
            .WithMany(sk => sk.Parts)
            .UsingEntity<PartMemberDataModel>(
                pm => pm
                    .HasOne(fk => fk.Member)
                    .WithMany(sk => sk.PartLinks)
                    .HasForeignKey(fk => fk.MemberId),
            pm => pm
                    .HasOne(fk => fk.Part)
                    .WithMany(sk => sk.PartMembers)
                    .HasForeignKey(fk => fk.PartId),
            pm =>
                    {
                        pm.Property(fk => fk.Privileges).HasDefaultValue(1);
                        pm.HasKey(k => new { k.MemberId, k.PartId });
                        pm.ToTable("PartMembers");
                    }
        );
        
        modelBuilder.Entity<PartDataModel>()
            .HasOne(fk => fk.MainPart)
            .WithMany(sk => sk.Parts)
            .HasForeignKey(fk => fk.MainPartId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TaskDataModel>()
            .HasOne(fk => fk.CurrentPart)
            .WithMany(fk => fk.Tasks)
            .HasForeignKey(t => t.PartId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TaskDataModel>()
            .HasMany(fk => fk.Members)
            .WithMany(sk => sk.Tasks)
            .UsingEntity<TaskMember>(
                pm => pm
                    .HasOne(fk => fk.Member)
                    .WithMany(sk => sk.MemberTasks)
                    .HasForeignKey(fk => fk.MemberId),
                pm => pm
                    .HasOne(fk => fk.Task)
                    .WithMany(sk => sk.TaskMembers)
                    .HasForeignKey(fk => fk.TaskId),
                pm =>
                {
                    pm.Property(fk => fk.ParticipationPurpose).HasDefaultValue(0);
                    pm.HasKey(k => new { k.MemberId, k.TaskId });
                    pm.ToTable("TaskMembers");
                }
        );

        modelBuilder.Entity<PartTaskStatus>()
            .HasOne(fk => fk.Part)
            .WithMany(fk => fk.TaskStatuses)
            .HasForeignKey(fk => fk.PartId);

        modelBuilder.Entity<MemberTag>()
            .HasKey(k => new {k.MemberId, k.TagId});
        modelBuilder.Entity<PartMemberRole>()
            .HasKey(k => new { k.MemberId, k.PartRoleId, k.PartId });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var secretProvider = new SecretProvider();
        optionsBuilder.UseNpgsql(
            secretProvider.GetMainConnection()
        );
    }
}