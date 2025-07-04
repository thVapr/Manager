﻿using ManagerData.Constants;
using Microsoft.EntityFrameworkCore;
using ManagerData.DataModels.Authentication;

namespace ManagerData.Contexts;

public sealed class AuthenticationDbContext : DbContext
{
    public AuthenticationDbContext()
    {
        Database.Migrate();
    }

    public DbSet<UserDataModel> Users { get; set; } = null!;
    public DbSet<RoleDataModel> Roles { get; set; } = null!;
    public DbSet<UserRoleDataModel> UserRoles { get; set; } = null!;
    public DbSet<RefreshTokenDataModel> Tokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRoleDataModel>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRoleDataModel>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.Roles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRoleDataModel>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<RefreshTokenDataModel>()
            .ToTable("Tokens")
            .HasKey(t => t.Id);

        modelBuilder.Entity<RefreshTokenDataModel>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var secretProvider = new SecretProvider();
        optionsBuilder.UseNpgsql(
            secretProvider.GetAuthConnection()
        );
    }
}

