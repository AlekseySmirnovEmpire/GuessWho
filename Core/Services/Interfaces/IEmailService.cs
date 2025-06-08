using Core.Database.Email;

namespace Core.Services.Interfaces;

public interface IEmailService
{
    public void PutNewMessageInQueue(EmailSendingQueue data);

    public Task SendMessagesFromQueue();
}