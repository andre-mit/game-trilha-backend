using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts;

public class TrilhaContext : DbContext
{
    public TrilhaContext(DbContextOptions<TrilhaContext> options) : base(options)
    {
    }
}