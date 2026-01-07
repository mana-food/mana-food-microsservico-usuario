using ManaFood.Infrastructure.Configurations;
using ManaFood.Application.Configurations;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.ConfigurePersistenceApp();
builder.Services.ConfigureApplicationApp();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ManaFood API - User Service", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no campo abaixo usando o prefixo 'Bearer'. Exemplo: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddOpenApi();

// Injeção de dependências
builder.Services.AddTransient<UserValidationService>();

var app = builder.Build();

// Middlewares
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ManaFood API - User Service V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.Run();