var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region CorsConfiguration

const string corsPolicyName = "AllowAllOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(corsPolicyName);
app.MapControllers();

app.Run();
