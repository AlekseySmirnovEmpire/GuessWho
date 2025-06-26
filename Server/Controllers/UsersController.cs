using Core.Models.Users;
using Core.Server.Providers;
using Microsoft.AspNetCore.Mvc;
using Server.Filters;

namespace Server.Controllers;

[Route("/api/v1/[controller]")]
[JwtAuth]
public class UsersController(IAuthProvider provider, ILogger<UsersController> logger) 
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
                NickName = user.NickName,
                Role = user.Role,
                Rating = user.Rating
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
}