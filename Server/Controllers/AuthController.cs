using Core.Models.Auth;
using Core.Server.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Route("/api/v1/[controller]")]
public class AuthController(IAuthProvider authProvider) : BaseApiController
{
    [Route("login")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginModel? loginModel)
    {
        try
        {
            return Ok(authProvider.Login(loginModel));
        }
        catch (Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => Unauthorized(),
                InvalidDataException => Forbid(),
                _ => BadRequest(new
                {
                    error = ex.Message
                })
            };
        }
    }

    [Route("signup")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult SignUp([FromBody] SignUpModel? userModel)
    {
        try
        {
            return Ok(authProvider.SignUp(userModel));
        }
        catch (Exception ex)
        {
            return ex switch
            {
                InvalidDataException => StatusCode(StatusCodes.Status409Conflict, new
                {
                    my = true,
                    error = ex.Message
                }),
                _ => BadRequest(new
                {
                    my = false,
                    error = ex.Message
                })
            };
        }
    }

    [Route("logout")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Logout()
    {
        try
        {
            authProvider.Logout(HttpContext.Request.Headers.Authorization.ToString()
                .Replace("Bearer", string.Empty)
                .Trim());
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }
    
    [Route("refresh")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Refresh([FromBody] TokenModel model)
    {
        try
        {
            return Ok(authProvider.RefreshToken(model));
        }
        catch (Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => Unauthorized(),
                _ => BadRequest(new
                {
                    error = ex.Message
                })
            };
        }
    }
}