using Core.Models.Lobbies;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class LobbyHub(ILogger<LobbyHub> logger) : Hub
{
    public async Task CreateLobby(LobbyDtoModel lobbyId)
    {
        logger.LogInformation($"Creating lobby: '{lobbyId}'");
        await Clients.All.SendAsync("LobbyCreated", lobbyId);
    }

    public async Task CloseLobby(Guid lobbyId)
    {
        logger.LogInformation($"Closing lobby: {lobbyId}");
        await Clients.All.SendAsync("LobbyClosed", lobbyId);
    }

    public override async Task OnConnectedAsync()
    {
        logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }
}