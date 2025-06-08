using System.Text.Json.Serialization;

namespace Core.Models.Auth;

public class TokenModel
{
    [JsonPropertyName("access_token")]
    public string? Access { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? Refresh { get; set; }
}