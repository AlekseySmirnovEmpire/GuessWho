using Core.Models.GamePacks;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class GamePackHub(ILogger<GamePackHub> logger) : Hub
{
    public async Task CreateGamePack(GamePackDtoModel pack)
    {
        await Clients.All.SendAsync("GamePackCreated", pack);
    }

    public async Task DeactiveGamePack(long packId)
    {
        await Clients.All.SendAsync("GamePackClosed", packId);
    }

    public async Task ActiveGamePack(long packId)
    {
        await Clients.All.SendAsync("GamePackOpened", packId);
    }

    public async Task GamePackDeleted(long packId)
    {
        await Clients.All.SendAsync("GamePackDeleted", packId);
    }

    public override async Task OnConnectedAsync()
    {
        logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }
}