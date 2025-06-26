using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Models.Auth;

public class SignUpModel
{
    [Required, MinLength(4), MaxLength(255), JsonPropertyName("nickName")]
    public string NickName { get; init; }

    [Required, MinLength(6), MaxLength(16), JsonPropertyName("password")]
    public string Password { get; init; }

    [Required, EmailAddress, JsonPropertyName("email")]
    public string Email { get; init; }
}