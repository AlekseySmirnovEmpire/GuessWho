using Core.Managers;
using Core.Services.Interfaces;

namespace BackgroundJobs.Jobs;

public class EmailSendingJob(
    IMessageBusManager messageBusManager,
    ILogger<EmailSendingJob> logger,
    IEmailService emailService)
    : BaseJob(messageBusManager, logger, nameof(EmailSendingJob))
{
    protected override async Task ExecuteJob() => await emailService.SendMessagesFromQueue();

    public override string ToString() => nameof(EmailSendingJob);

    protected override void LogError(Exception ex) => logger.LogError(ex, "Ошибка");
}