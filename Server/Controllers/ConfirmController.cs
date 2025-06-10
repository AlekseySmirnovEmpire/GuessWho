using Core.Models.Users;
using Core.Server.Providers;
using Core.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Server.Filters;

namespace Server.Controllers;

[Route("/api/v1/[controller]")]
public class ConfirmController(UserService userService, ILogger<ConfirmController> logger, IAuthProvider authProvider) 
    : BaseApiController
{
    [Route("email")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ConfirmUser([FromBody] ConfirmModel? model)
    {
        try
        {
            return Ok(new { status = userService.ConfirmUserEmail(model?.Token) });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);

            return BadRequest(new { status = false });
        }
    }

    [Route("moderator")]
    [JwtAuth]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult ConfirmUser([FromQuery(Name = "userId")] long userId)
    {
        try
        {
            return Ok(new
            {
                status = userService.ConfirmUserByModerator(userId, authProvider.GetCurrentUser(HttpContext))
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);

            return ex switch
            {
                UnauthorizedAccessException => Forbid(),
                _ => BadRequest(new { status = false })
            };
        }
    }
}