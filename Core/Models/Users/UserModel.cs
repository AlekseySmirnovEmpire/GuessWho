using System.Text.Json.Serialization;

namespace Core.Models.Users;

public class UserModel
{
    [JsonPropertyName("nickName")] public string NickName { get; init; }

    [JsonPropertyName("role")] public UserRole Role { get; init; }

    [JsonPropertyName("rating")] public ushort Rating { get; init; }
}