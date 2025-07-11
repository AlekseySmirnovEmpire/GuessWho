using Core.Models.Lobbies;
using Core.Server.Providers;
using Core.Server.Services.Lobbies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Filters;
using Server.Hubs;

namespace Server.Controllers;

[Route("/api/v1/[controller]")]
[JwtAuth]
public class LobbiesController(
    LobbyService lobbyService,
    ILogger<LobbiesController> logger,
    IAuthProvider provider,
    IHubContext<LobbyHub> hubContext) : BaseApiController
{
    [HttpPut]
    public async Task<IActionResult> Create([FromBody] LobbyDtoModel lobby)
    {
        try
        {
            var host = provider.GetCurrentUser(HttpContext);
            lobby.HostId = host.Id;
            var created = lobbyService.CreateLobby(lobby, host);
            await hubContext.Clients.All.SendAsync("LobbyCreated", created);

            return Ok(new
            {
                id = created.Id
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(lobbyService.FindAll());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        try
        {
            return Ok(lobbyService.Find(id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return BadRequest();
        }
    }

    [HttpPost]
    [Route("{lobbyId:guid}/password")]
    public IActionResult CheckPassword([FromBody] PasswordRequestBody password, Guid lobbyId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            lobbyService.CheckPassword(password.Password, lobbyId);

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }
}