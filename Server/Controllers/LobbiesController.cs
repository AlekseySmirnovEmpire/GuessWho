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
            lobby.HostId = provider.GetCurrentUser(HttpContext).Id;
            var created = lobbyService.CreateLobby(lobby);
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
}