﻿using GameTrilha.API.Contexts.Seeds;
using GameTrilha.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts;

public class TrilhaContext : DbContext
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Skin> Skins { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<RecoveryPasswordCode> RecoveryPasswordCodes { get; set; }

    public TrilhaContext(DbContextOptions<TrilhaContext> options) : base(options) { }
    
    // TODO: Move settings to assembly configuration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasMany(u => u.Skins).WithMany(s => s.Users);
        modelBuilder.Entity<User>().HasOne(u => u.Skin).WithMany(s => s.UsersOwn);
        modelBuilder.Entity<User>().HasMany(u => u.Boards).WithMany(b => b.Users);
        modelBuilder.Entity<User>().HasOne(u => u.Board).WithMany(b => b.UsersOwn);
        modelBuilder.Entity<User>().HasMany(u => u.MatchesPlayer1).WithOne(m => m.Player1);
        modelBuilder.Entity<User>().HasMany(u => u.MatchesPlayer2).WithOne(m => m.Player2);
        modelBuilder.Entity<User>().HasMany(u => u.Wins).WithOne(m => m.Winner);
        modelBuilder.Entity<User>().HasIndex(u=>u.Email).IsUnique();

        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<User>().OwnsOne(user => user.Avatar, ownedNavigationBuilder =>
        {
            ownedNavigationBuilder.ToJson();
        });

        modelBuilder.AddRoles();
        modelBuilder.AddUsers();
        modelBuilder.LinkAdminUserRole();
    }
}