using ManaFood.Application.Constants;
using ManaFood.Application.Interfaces.Services;
using ManaFood.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

        // Autenticação JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    // Transformar claims para remover namespaces
                    if (context.Principal?.Identity is ClaimsIdentity identity)
                    {
                        var claimsToAdd = new List<Claim>();
                        var claimsToRemove = new List<Claim>();

                        // Mapear role claim
                        var roleClaim = identity.FindFirst(c => 
                            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" ||
                            c.Type == ClaimTypes.Role);
                        if (roleClaim != null)
                        {
                            claimsToRemove.Add(roleClaim);
                            claimsToAdd.Add(new Claim("role", roleClaim.Value));
                        }

                        // Mapear email claim
                        var emailClaim = identity.FindFirst(c => 
                            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" ||
                            c.Type == ClaimTypes.Email);
                        if (emailClaim != null)
                        {
                            claimsToRemove.Add(emailClaim);
                            claimsToAdd.Add(new Claim("email", emailClaim.Value));
                        }

                        // Mapear sub claim
                        var subClaim = identity.FindFirst(c => 
                            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" ||
                            c.Type == ClaimTypes.NameIdentifier);
                        if (subClaim != null)
                        {
                            claimsToRemove.Add(subClaim);
                            claimsToAdd.Add(new Claim("sub", subClaim.Value));
                        }

                        // Remover claims antigos e adicionar novos
                        foreach (var claim in claimsToRemove)
                        {
                            identity.RemoveClaim(claim);
                        }
                        foreach (var claim in claimsToAdd)
                        {
                            identity.AddClaim(claim);
                        }
                    }

                    return Task.CompletedTask;
                }
            };
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "role",
                NameClaimType = "name"
            };
        });

        // Roles e Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.ADMIN_ONLY, policy =>
                policy.RequireRole(Roles.ADMIN));

            options.AddPolicy(Policies.ADMIN_OR_MANAGER, policy =>
                policy.RequireRole(Roles.ADMIN, Roles.MANAGER));

            options.AddPolicy(Policies.KITCHEN_STAFF, policy =>
                policy.RequireRole(Roles.KITCHEN, Roles.ADMIN, Roles.MANAGER));

            options.AddPolicy(Policies.OPERATORS, policy =>
                policy.RequireRole(Roles.OPERATOR, Roles.ADMIN, Roles.MANAGER));

            options.AddPolicy(Policies.MANAGEMENT, policy =>
                policy.RequireRole(Roles.ADMIN, Roles.MANAGER));

            options.AddPolicy(Policies.AUTHENTICATED_USER, policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy(Policies.ORDER_MANAGEMENT, policy =>
                policy.RequireRole(Roles.ADMIN, Roles.KITCHEN));
        
            options.AddPolicy(Policies.DATA_QUERY, policy =>
                policy.RequireRole(Roles.ADMIN, Roles.MANAGER, Roles.OPERATOR));
        });
    }
}