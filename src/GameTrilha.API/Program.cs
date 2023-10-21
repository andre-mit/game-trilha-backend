using System.Text;
using GameTrilha.API.Contexts;
using GameTrilha.API.Hubs;
using GameTrilha.API.Services;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.SetupConfigurations;
using GameTrilha.API.SetupConfigurations.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationInsightsTelemetry();

#region CorsConfiguration

const string corsPolicyName = "AllowAllOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policyBuilder =>
    {
        policyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

#endregion

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityFrameworkSqlServer()
    .AddDbContext<TrilhaContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SQLSERVER_Trilha"));
    });

#region JWT

var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtOptions>(jwtSection);
var key = jwtSection.Get<JwtOptions>()!.Key;

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication(key);

builder.Services.AddScoped<IAuthService, AuthService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors(corsPolicyName);
app.MapHub<GameHub>("/game");

app.Run();
