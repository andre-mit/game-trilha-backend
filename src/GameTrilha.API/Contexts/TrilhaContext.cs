using GameTrilha.API.Contexts.Seeds;
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

    public TrilhaContext(DbContextOptions<TrilhaContext> options) : base(options) { }

    // Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasMany(u => u.Skins).WithMany(s => s.Users);
        modelBuilder.Entity<User>().HasMany(u => u.Boards).WithMany(b => b.Users);
        modelBuilder.Entity<User>().HasMany(u => u.MatchesPlayer1).WithOne(m => m.Player1);
        modelBuilder.Entity<User>().HasMany(u => u.MatchesPlayer2).WithOne(m => m.Player2);
        modelBuilder.Entity<User>().HasMany(u => u.Wins).WithOne(m => m.Winner);

        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.AddRoles();
        modelBuilder.AddUsers();
        modelBuilder.LinkAdminUserRole();
    }
}