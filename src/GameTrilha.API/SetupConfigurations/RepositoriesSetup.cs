using GameTrilha.API.Contexts.Repositories;
using GameTrilha.Domain.Entities.Repositories;

namespace GameTrilha.API.SetupConfigurations;

public static class RepositoriesSetup
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<ISkinRepository, SkinRepository>();
    }
}