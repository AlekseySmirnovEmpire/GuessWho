using System.Net;
using System.Net.Mail;
using Core.Server.Database.Email;
using Core.Server.Repositories;
using Core.Server.Services.Interfaces;
using Core.Settings;
using Microsoft.Extensions.Logging;

namespace Core.Server.Services;

public class SmtpEmailService(
    IEmailSendingQueueRepository repository, 
    ILogger<SmtpEmailService> logger, 
    EmailSettings emailSettings)
    : IEmailService
{
    public void PutNewMessageInQueue(EmailSendingQueue data)
    {
        ArgumentNullException.ThrowIfNull(data);

        repository.Add(data);
    }

    public async Task SendMessagesFromQueue()
    {
        var dataToSend = repository
            .FindList(esq => !esq.SendAt.HasValue && !esq.WithError)
            .OrderBy(esq => esq.Priority)
            .Take(5)
            .ToList();

        var updated = new List<Guid>(5);
        foreach (var data in dataToSend)
        {
            try
            {
                await Send(data);
                updated.Add(data.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка отправки письма '{0}'", data.Id);
            }
        }

        if (updated.Count != 0)
            repository.Update(
                esq => updated.Contains(esq.Id),
                sp => sp.SetProperty(esq => esq.SendAt, DateTime.Now));

        var withErrors = dataToSend.ExceptBy(updated, esq => esq.Id).Select(esq => esq.Id).ToList();
        if (withErrors.Count != 0)
            repository.Update(
                esq => withErrors.Contains(esq.Id),
                sp => sp.SetProperty(esq => esq.WithError, true));
    }

    private async Task Send(EmailSendingQueue data)
    {
        using var smtpClient = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort);
        smtpClient.EnableSsl = true;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(
            emailSettings.SmtpLogin,
            emailSettings.SmtpPassword);

        using var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(emailSettings.SmtpLogin, "Guess Who");
        mailMessage.Subject = data.Title;
        mailMessage.Body = data.Body;
        mailMessage.IsBodyHtml = true;
        mailMessage.Priority = MailPriority.Normal;

        mailMessage.To.Add(data.EmailTo);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            logger.LogInformation("Email sent to {0}", data.EmailTo);
        }
        catch (SmtpException ex)
        {
            logger.LogError(ex, "SMTP error sending email to {0}", data.EmailTo);

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending email to {0}", data.EmailTo);

            throw;
        }
    }
}