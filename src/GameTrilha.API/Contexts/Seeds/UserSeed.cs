using GameTrilha.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts.Seeds;

public static class UserSeed
{
    static readonly Guid id = Guid.Parse("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a");
    public static void AddUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<User>()
            .HasData(
                new User
                {
                    Id = id,
                    Name = "Administrador",
                    Email = "admin@trilha.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admininastro")
                });
    }

    public static void LinkAdminUserRole(this ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserRole>()
            .HasData(
                new UserRole
                {
                    UserId = id,
                    RoleId = id
                });

        modelBuilder
            .Entity<UserRole>()
            .HasData(
                new UserRole
                {
                    UserId = id,
                    RoleId = Guid.Parse("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1b")
                });
    }
}