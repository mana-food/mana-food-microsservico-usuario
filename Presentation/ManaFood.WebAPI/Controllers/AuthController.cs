using ManaFood.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ManaFood.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthAppService authAppService) : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await authAppService.Login(request, cancellationToken);
            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(new { Token = result.Token });
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = authAppService.Logout(token);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpGet("me")]
        [AllowAnonymous]
        public IActionResult GetCurrentUser()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = authAppService.GetCurrentUser(token);
            if (!result.Success)
                return Unauthorized();

            return Ok(new { Email = result.Email });
        }
    }
}