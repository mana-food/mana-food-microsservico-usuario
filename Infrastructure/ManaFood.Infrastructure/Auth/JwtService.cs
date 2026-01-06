using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManaFood.Application.Interfaces.Services;
using ManaFood.Application.Mappings;
using ManaFood.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace ManaFood.Infrastructure.Auth;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly int _expirationMinutes;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly ITokenBlacklistService _blacklistService;

    public JwtService(string secretKey, int expirationMinutes, string issuer, string audience, ITokenBlacklistService blacklistService)
    {
        _secretKey = secretKey;
        _expirationMinutes = expirationMinutes;
        _issuer = issuer;
        _audience = audience;
        _blacklistService = blacklistService;
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("role", UserTypeMapper.ToRoleString(user.UserType)), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        if (_blacklistService.IsBlacklisted(token))
            return null;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public void InvalidateToken(string token)
    {
        _blacklistService.Add(token);
    }
}