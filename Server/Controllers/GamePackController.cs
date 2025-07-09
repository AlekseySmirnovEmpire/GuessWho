using Core.Models.GamePacks;
using Core.Server.Providers;
using Core.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Filters;
using Server.Hubs;

namespace Server.Controllers;

[Route("/api/v1/[controller]")]
public class GamePackController(
    GamePackService service,
    ILogger<GamePackController> logger,
    IAuthProvider provider,
    IHubContext<GamePackHub> hubContext)
    : BaseApiController
{
    [HttpPut]
    [JwtAuth]
    public async Task<IActionResult> Create([FromBody] GamePackCreateDtoModel model)
    {
        try
        {
            var data = service.Create(model, provider.GetCurrentUser(HttpContext));
            await hubContext.Clients.All.SendAsync("GamePackCreated", data);

            return Ok(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (ex is UnauthorizedAccessException) return Forbid();

            return BadRequest();
        }
    }

    [HttpPost]
    [Route("change-activity/{id:long}")]
    [JwtAuth]
    public async Task<IActionResult> ChangeGamePackActivity(long id, [FromQuery(Name = "active")] bool active)
    {
        try
        {
            service.ChangeGamePackActivity(
                provider.GetCurrentUser(HttpContext),
                id,
                active);
            await hubContext.Clients.All.SendAsync(active ? "GamePackOpened" : "GamePackClosed", id);

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (ex is UnauthorizedAccessException) return Forbid();

            return BadRequest();
        }
    }

    [HttpDelete]
    [Route("{id:long}")]
    [JwtAuth]
    public async Task<IActionResult> Remove(long id)
    {
        try
        {
            service.Remove(provider.GetCurrentUser(HttpContext), id);
            await hubContext.Clients.All.SendAsync("GamePackDeleted", id);

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
    public IActionResult GetAll([FromQuery(Name = "active")] bool needActivity = false)
    {
        try
        {
            return Ok(service.FindAll(needActivity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (ex is UnauthorizedAccessException) return Forbid();

            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{id:long}")]
    [JwtAuth]
    public IActionResult Get(long id)
    {
        try
        {
            return Ok(service.Find(id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (ex is ArgumentNullException) return NotFound();

            return BadRequest();
        }
    }
}