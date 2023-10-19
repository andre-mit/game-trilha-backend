using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GameTrilha.API.SetupConfigurations;

public static class JwtSetup
{
    /// <summary>
    /// Inject JWT configuration
    /// </summary>
    /// <param name="builder">Web Application builder instance</param>
    public static void AddJwtAuthentication(this IServiceCollection services, string keyString)
    {
        var key = Encoding.ASCII.GetBytes(keyString);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }
}