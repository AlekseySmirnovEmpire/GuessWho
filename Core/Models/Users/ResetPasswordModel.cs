using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Models.Users;

public class ResetPasswordModel
{
    [Required, JsonPropertyName("token")] public string Token { get; set; }

    [Required, MinLength(6), MaxLength(30), JsonPropertyName("password")]
    public string Password { get; set; }
}