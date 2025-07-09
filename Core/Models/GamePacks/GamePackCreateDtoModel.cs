using System.Text.Json.Serialization;

namespace Core.Models.GamePacks;

public class GamePackCreateDtoModel
{
    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("data")] public byte[] Data { get; set; }
}