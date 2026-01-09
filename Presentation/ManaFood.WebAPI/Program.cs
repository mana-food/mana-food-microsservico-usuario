using ManaFood.Infrastructure.Configurations;
using ManaFood.Application.Configurations;
using ManaFood.Application.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigurePersistenceApp();
builder.Services.ConfigureApplicationApp();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ManaFood API - User Service", Version = "v1" });
});

builder.Services.AddOpenApi();

// Injeção de dependências
builder.Services.AddTransient<UserValidationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ManaFood API - User Service V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

await app.RunAsync();