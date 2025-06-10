namespace Core.Server.Services.Interfaces;

public interface IEmailTemplatingService
{
    public string GenerateEmailTemplate<T>(SubstitutionEmailTemplates templateName, T model);
}