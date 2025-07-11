using Core.Server.Managers;
using Core.Server.Services.Lobbies;

namespace BackgroundJobs.Jobs;

public class OldLobbiesCleanerJob(IMessageBusManager messageBusManager, ILogger<BaseJob> logger, LobbyService lobbyService)
    : BaseJob(messageBusManager, logger, nameof(OldLobbiesCleanerJob))
{
    private readonly ILogger<BaseJob> _logger = logger;

    protected override async Task ExecuteJob() => await lobbyService.RemoveOld();

    public override string ToString() => nameof(OldLobbiesCleanerJob);

    protected override void LogError(Exception ex) => _logger.LogError(ex, ex.Message);
}