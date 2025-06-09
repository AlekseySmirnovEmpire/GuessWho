using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Core.Database.Users;
using Core.Models.Users;

namespace Core.Services;

public static class EmailVerificationService
{
    private static readonly string SecretKey =
        Environment.GetEnvironmentVariable("ASPNETCORE_EMAIL_SECRET_KEY") ?? throw new ArgumentNullException();

    public static string GenerateVerificationLink(User user)
    {
        var claims = new UserClaims(user);

        // Сериализация модели
        var json = JsonSerializer.Serialize(claims);

        // Создание подписи
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(json)));

        // Объединение данных и подписи
        var payload = $"{json}.{signature}";

        // Кодирование в Base64Url
        return $"{Environment.GetEnvironmentVariable("CLIENT_URL")}/confirm?token={Base64UrlEncode(payload)}";
    }

    public static UserClaims? VerifyLink(string token)
    {
        // Декодирование из Base64Url
        var decoded = Base64UrlDecode(token);

        // Разделение данных и подписи
        var parts = decoded.Split('.');
        if (parts.Length != 2)
            throw new Exception("Неверный формат токена");

        var json = parts[0];
        var signature = parts[1];

        // Проверка подписи
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
        var computedSignature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(json)));

        if (!SignatureMatches(signature, computedSignature))
            throw new Exception("Подпись не совпадает");

        // Десериализация модели
        return JsonSerializer.Deserialize<UserClaims>(json);
    }

    private static bool SignatureMatches(string signature, string computedSignature) =>
        Convert.FromBase64String(signature).SequenceEqual(Convert.FromBase64String(computedSignature));

    private static string Base64UrlEncode(string input) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(input))
            .Replace('/', '_')
            .Replace('+', '-')
            .TrimEnd('=');

    private static string Base64UrlDecode(string input) =>
        Encoding.UTF8.GetString(
            Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/') + "=="[..(input.Length % 4)]));
}