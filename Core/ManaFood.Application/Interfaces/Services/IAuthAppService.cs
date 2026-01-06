using Microsoft.AspNetCore.Identity.Data;

namespace ManaFood.Application.Interfaces.Services
{
    public interface IAuthAppService
    {
        Task<AuthResult> Login(LoginRequest request, CancellationToken cancellationToken);
        AuthResult Logout(string token);
        AuthResult GetCurrentUser(string token);
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}

