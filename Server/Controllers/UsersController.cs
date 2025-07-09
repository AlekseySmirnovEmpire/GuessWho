using Core.Models.Users;
using Core.Server.Database.Users;
using Core.Server.Providers;
using Core.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Server.Filters;

namespace Server.Controllers;

[Route("/api/v1/[controller]")]
[JwtAuth]
public class UsersController(IAuthProvider provider, ILogger<UsersController> logger, UserService service)
    : BaseApiController
{
    [HttpGet]
    public IActionResult GetUser()
    {
        try
        {
            var user = provider.GetCurrentUser(HttpContext);
            return Ok(new UserModel
            {
                Id = user.Id,
                NickName = user.NickName,
                Role = user.Role,
                Rating = user.Rating,
                ImageId = user.ImageId
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return ex switch
            {
                _ => BadRequest()
            };
        }
    }

    [HttpPost]
    public IActionResult ChangeData([FromBody] ChangeUserData data)
    {
        try
        {
            var user = provider.GetCurrentUser(HttpContext);
            service.ChangeUserData(data, user.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return ex switch
            {
                _ => BadRequest()
            };
        }
    }

    [HttpGet]
    [Route("rating")]
    public IActionResult GetUsersRating()
    {
        try
        {
            var user = provider.GetCurrentUser(HttpContext);
            var users = service.FindAllForRating();
            return Ok(users.Select(u => new UserModel
            {
                NickName = u.NickName,
                Rating = u.Rating,
                ActiveUser = user.Id == u.Id
            }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return ex switch
            {
                _ => BadRequest()
            };
        }
    }

    [HttpPost]
    [Route("confirm/{userId:long}")]
    public IActionResult ConfirmByModerator(long userId)
    {
        try
        {
            if (!service.ConfirmUserByModerator(userId, provider.GetCurrentUser(HttpContext))) return BadRequest();

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (ex is UnauthorizedAccessException) return Forbid();

            return BadRequest();
        }
    }

    [HttpPost]
    [Route("ban/{userId:long}")]
    public IActionResult ChangeUserBanStatus(long userId)
    {
        try
        {
            if (!service.ChangeUserBanStatus(provider.GetCurrentUser(HttpContext), userId)) return BadRequest();

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (ex is UnauthorizedAccessException) return Forbid();

            return BadRequest();
        }
    }

    [HttpGet]
    [Route("all")]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(service.FindAllUsers(provider.GetCurrentUser(HttpContext)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (ex is UnauthorizedAccessException) return Forbid();

            return BadRequest();
        }
    }
}