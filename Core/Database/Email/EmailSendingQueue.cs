using System.ComponentModel.DataAnnotations;

namespace Core.Database.Email;

public class EmailSendingQueue
{
    [Key] public Guid Id { get; init; } = Guid.NewGuid();

    [Required, MinLength(6), MaxLength(255)]
    public string Title { get; init; }

    [Required] public string EmailTo { get; init; }

    public string Body { get; init; }

    public EmailPriority Priority { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? SendAt { get; init; }

    public bool WithError { get; set; }

    public EmailSendingQueue(string title, string body, string email, EmailPriority priority)
    {
        Title = title;
        EmailTo = email;
        Body = body;
        Priority = priority;
        CreatedAt = DateTime.Now;
    }

    [Obsolete(message: "ONLY EF", error: true)]
    public EmailSendingQueue()
    {
    }
}