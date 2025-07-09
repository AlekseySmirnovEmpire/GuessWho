using Core.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Filters;

public class JwtAuthFilter(IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
    : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString()
            .Replace("Bearer", string.Empty)
            .Trim();

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();

            return;
        }

        var user = tokenService.Validate(token);
        if (user == null)
        {
            context.Result = new UnauthorizedResult();

            return;
        }

        // Добавляем пользователя в контекст запроса
        try
        {
            httpContextAccessor.HttpContext!.Items["AuthenticatedUser"] = user;
        }
        catch
        {
            //
        }
    }
}