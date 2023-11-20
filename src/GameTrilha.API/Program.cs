using System;
using System.Text;
using Azure.Storage.Blobs;
using GameTrilha.API.Contexts;
using GameTrilha.API.Contexts.Repositories;
using GameTrilha.API.Helpers.JsonConverters;
using GameTrilha.API.Hubs;
using GameTrilha.API.Services;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.SetupConfigurations;
using GameTrilha.API.SetupConfigurations.Models;
using GameTrilha.API.ViewModels.GameViewModels;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using Resend;

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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new MultidimensionalObjectArrayJsonConverter<PlaceViewModel>());
    });

builder.Services.AddSignalR();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityFrameworkSqlServer()
    .AddDbContext<TrilhaContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SQLSERVER_Trilha"));
    });

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddRepositories();

builder.Services.AddServices();

builder.Services.AddTransient(_ => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));

#region Email

builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
var emailOptions = builder.Configuration.GetSection("EmailOptions");
builder.Services.Configure<EmailClientOptions>(emailOptions);
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = emailOptions.Get<EmailClientOptions>()!.ApiToken;
});
builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();

#endregion

#region JWT

var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtOptions>(jwtSection);
var key = jwtSection.Get<JwtOptions>()!.Key;

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication(key);

#endregion

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<TrilhaContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/game");
app.UseCors(corsPolicyName);

app.Run();
