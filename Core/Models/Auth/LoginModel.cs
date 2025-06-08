using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Models.Auth;

public class LoginModel
{
    [Required, EmailAddress, JsonPropertyName("email")]
    public string Email { get; init; }

    [Required, MinLength(6), MaxLength(30), JsonPropertyName("password")]
    public string Password { get; init; }

    [JsonPropertyName("rememberMe")] public bool RememberMe { get; init; }
}