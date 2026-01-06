using ManaFood.Application.Interfaces.Services;

namespace ManaFood.WebAPI.Middlewares;

public class JwtAuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        // Ignora requisições OPTIONS (CORS pré-flight)
        if (context.Request.Method == HttpMethods.Options)
        {
            await context.Response.WriteAsync("");
            return;
        }

        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(token))
        {
            var principal = jwtService.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token inválido, expirado ou em blacklist.");
                return;
            }
        }
        await next(context);
    }
}