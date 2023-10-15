using GameTrilha.API.Contexts;
using GameTrilha.API.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);
app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/game");

app.Run();
