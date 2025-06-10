using Core.Server.Database.Email;

namespace Core.Server.Services.Interfaces;

public interface IEmailService
{
    public void PutNewMessageInQueue(EmailSendingQueue data);

    public Task SendMessagesFromQueue();
}