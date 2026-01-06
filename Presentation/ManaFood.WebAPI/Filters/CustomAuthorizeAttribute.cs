using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManaFood.WebAPI.Filters;

public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public CustomAuthorizeAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles ?? Array.Empty<string>();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        if (!user.Identity?.IsAuthenticated == true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_allowedRoles.Length == 0)
            return; 

        var userRole = user.FindFirst("role")?.Value;
        
        if (string.IsNullOrEmpty(userRole) || !_allowedRoles.Contains(userRole))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}