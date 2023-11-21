using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.Services;

namespace GameTrilha.API.SetupConfigurations;

public static class ServicesSetup
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMatchService, MatchService>();

        services.AddTransient<IFileStorageService, FileStorageService>();
    }
}