using System.Security.Claims;
using ManaFood.Domain.Entities;

namespace ManaFood.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal ValidateToken(string token);
    void InvalidateToken(string token);
}