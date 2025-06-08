namespace Core.Settings;

public class EmailSettings
{
    public readonly string SmtpHost = Environment.GetEnvironmentVariable("ASPNETCORE_EMAIL_SMTP_HOST") ??
                                      throw new ArgumentNullException();
    public readonly string SmtpLogin = Environment.GetEnvironmentVariable("ASPNETCORE_EMAIL_SMTP_LOGIN") ??
                                       throw new ArgumentNullException();
    public readonly string SmtpPassword = Environment.GetEnvironmentVariable("ASPNETCORE_EMAIL_SMTP_PASSWORD") ??
                                          throw new ArgumentNullException();
    public readonly int SmtpPort =
        int.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_EMAIL_SMTP_PORT"), out var port)
            ? port
            : throw new ArgumentNullException();
}