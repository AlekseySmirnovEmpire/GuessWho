using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Models.Users;

public class ConfirmModel
{
    [Required]
    [JsonPropertyName("token")]
    public string Token { get; init; }
}