using System.Text.Json.Serialization;
using Core.Models.Users;

namespace Core.Models.Lobbies;

public class LobbyDtoModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("hostId")]
    public long HostId { get; set; }

    [JsonPropertyName("host")]
    public UserModel Host { get; set; }

    [JsonPropertyName("playersCount")]
    public ushort PlayersCount { get; set; }

    [JsonPropertyName("ratingChange")]
    public ushort RatingChange { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("fileId")]
    public long GamePackId { get; set; }

    [JsonPropertyName("animation")]
    public string AnimationClass { get; set; }

    [JsonPropertyName("conditions")]
    public List<LobbyCondition> Conditions { get; set; } = [];

    [JsonPropertyName("users")]
    public ICollection<UserModel> Users { get; set; }
}