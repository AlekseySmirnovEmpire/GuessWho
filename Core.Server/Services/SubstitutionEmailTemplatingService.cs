using Core.Server.Database.Users;
using Core.Server.Services.Interfaces;

namespace Core.Server.Services;

public class SubstitutionEmailTemplatingService : IEmailTemplatingService
{
    private static readonly Dictionary<SubstitutionEmailTemplates, string> Templates =
        new()
        {
            [SubstitutionEmailTemplates.ConfirmEmail] = "ConfirmEmailTemplate.html",
            [SubstitutionEmailTemplates.ResetPassword] = "ResetPasswordTemplate.html",
        };

    public string GenerateEmailTemplate<T>(SubstitutionEmailTemplates templateName, T model)
    {
        // Получаем путь к файлу шаблона
        var templateFileName = Templates[templateName];

        // Определяем базовый путь к папке с шаблонами
        var templatesFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Core", "Templates");

        // Полный путь к файлу шаблона
        var templatePath = Path.Combine(templatesFolder, templateFileName);

        // Проверяем существование файла
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Email template not found: {templatePath}");

        // Читаем содержимое шаблона
        var template = File.ReadAllText(templatePath);

        // Заменяем плейсхолдеры на реальные значения
        return templateName switch
        {
            SubstitutionEmailTemplates.ConfirmEmail when model is User userModel => template
                .Replace("{{UserName}}", userModel.NickName)
                .Replace("{{ConfirmationLink}}", EmailVerificationService.GenerateVerificationLink(userModel))
                .Replace("{{LogoUrl}}", "https://example.com/logo.png")
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString()),
            SubstitutionEmailTemplates.ResetPassword when model is User userModel => template
                .Replace("{{UserName}}", userModel.NickName)
                .Replace("{{ResetPasswordLink}}", EmailVerificationService.GenerateResetPasswordLink(userModel))
                .Replace("{{LogoUrl}}", "https://example.com/logo.png")
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString()),
            _ => throw new ArgumentException($"Unsupported template or model type: {templateName}")
        };
    }
}

public enum SubstitutionEmailTemplates
{
    ConfirmEmail,
    ResetPassword
}