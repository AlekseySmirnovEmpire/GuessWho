using Core.Server.Managers;
using Core.Server.Services;

namespace BackgroundJobs.Jobs;

public class UserCleanerJob(
    IMessageBusManager messageBusManager,
    ILogger<UserCleanerJob> logger,
    UserService userService)
    : BaseJob(messageBusManager, logger, nameof(UserCleanerJob))
{
    protected override Task ExecuteJob()
    {
        userService.RemoveOld();

        return Task.CompletedTask;
    }

    public override string ToString() => nameof(UserCleanerJob);

    protected override void LogError(Exception ex) => logger.LogError(ex, ex.Message);
}