using GameTrilha.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts.Seeds;

public static class RoleSeed
{
    public static void AddRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Role>()
            .HasData(
                new Role { Id = Guid.Parse("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"), Name = "Admin" },
                new Role { Id = Guid.Parse("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1b"), Name = "User" });
    }
}