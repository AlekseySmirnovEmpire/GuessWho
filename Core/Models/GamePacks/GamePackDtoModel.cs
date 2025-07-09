using System.Text.Json.Serialization;
using Core.Models.Users;

namespace Core.Models.GamePacks;

public class GamePackDtoModel
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("userCreatedId")] public long UserCreatedId { get; set; }

    [JsonPropertyName("userCreated")] public UserModel UserCreated { get; set; }

    [JsonPropertyName("isActive")] public bool IsActive { get; set; }
}