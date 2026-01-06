using ManaFood.Application.Interfaces;
using ManaFood.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity.Data;

namespace ManaFood.Application.Services
{
    public class AuthAppService(
        IUserRepository userRepository,
        IJwtService jwtService,
        ITokenBlacklistService tokenBlacklistService)
        : IAuthAppService
    {
        public async Task<AuthResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return new AuthResult { Success = false, Message = "Email e senha são obrigatórios." };

            var user = await userRepository.GetBy(u => u.Email == request.Email && u.Password == request.Password, cancellationToken);
            if (user == null)
                return new AuthResult { Success = false, Message = "Email ou senha inválidos." };

            var token = jwtService.GenerateToken(user);
            return new AuthResult { Success = true, Token = token };
        }

        public AuthResult Logout(string token)
        {
            if (string.IsNullOrEmpty(token))
                return new AuthResult { Success = false, Message = "Token não informado." };

            tokenBlacklistService.Add(token);
            return new AuthResult { Success = true, Message = "Logout realizado com sucesso." };
        }

        public AuthResult GetCurrentUser(string token)
        {
            var principal = jwtService.ValidateToken(token);
            if (principal == null)
                return new AuthResult { Success = false, Message = "Token inválido ou expirado." };

            var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            return new AuthResult { Success = true, Email = email };
        }
    }
}

