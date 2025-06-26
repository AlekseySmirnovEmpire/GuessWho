using System.Text.Json.Serialization;

namespace Core.Models.Auth;

public class UserCreateResponseModel
{
    [JsonPropertyName("error")]
    public string Error { get; set; }
}