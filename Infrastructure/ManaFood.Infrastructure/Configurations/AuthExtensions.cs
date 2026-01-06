using ManaFood.Application.Constants;
using ManaFood.Application.Interfaces.Services;
using ManaFood.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace ManaFood.Infrastructure.Configurations;

public static class AuthExtensions
{
    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        // JWT & Auth services
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
        services.AddSingleton<IJwtService>(provider =>
        {
            var expiration = int.Parse(jwtSettings["ExpirationMinutes"]);
            var blacklistService = provider.GetRequiredService<ITokenBlacklistService>();
            return new JwtService(secretKey, expiration, issuer, audience, blacklistService);
        });

        // Autenticação JWT - USAR ESQUEMAS PADRÃO
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero // Evita problemas de tempo
            };
        });

        // Roles e Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.ADMIN_ONLY, policy =>
                policy.RequireClaim(ClaimTypes.Role, Roles.ADMIN));

            options.AddPolicy(Policies.ADMIN_OR_MANAGER, policy =>
                policy.RequireClaim(ClaimTypes.Role, Roles.ADMIN, Roles.MANAGER));

            options.AddPolicy(Policies.KITCHEN_STAFF, policy =>
                policy.RequireClaim(ClaimTypes.Role, Roles.KITCHEN, Roles.ADMIN, Roles.MANAGER));

            options.AddPolicy(Policies.OPERATORS, policy =>
                policy.RequireClaim(ClaimTypes.Role, Roles.OPERATOR, Roles.ADMIN, Roles.MANAGER));

            options.AddPolicy(Policies.MANAGEMENT, policy =>
                policy.RequireClaim(ClaimTypes.Role, Roles.ADMIN, Roles.MANAGER));

            options.AddPolicy(Policies.AUTHENTICATED_USER, policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy(Policies.ORDER_MANAGEMENT, policy =>
                policy.RequireClaim(ClaimTypes.Role, Roles.ADMIN, Roles.KITCHEN));
        
            options.AddPolicy(Policies.DATA_QUERY, policy =>
                policy.RequireClaim(ClaimTypes.Role, Roles.ADMIN, Roles.MANAGER, Roles.OPERATOR));
        
        });
    }
}