using System.Text.Json.Serialization;

namespace Core.Models.Users;

public class UserModel
{
    [JsonPropertyName("id")] public long Id { get; init; }

    [JsonPropertyName("nickName")] public string NickName { get; init; }

    [JsonPropertyName("role")] public UserRole Role { get; init; }

    [JsonPropertyName("rating")] public ushort Rating { get; init; }

    [JsonPropertyName("avatarId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? ImageId { get; set; }

    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("emailConfirm")] public bool ConfirmEmail { get; set; }

    [JsonPropertyName("moderatorConfirm")] public bool ConfirmByModerator { get; set; }

    [JsonPropertyName("active")] public bool ActiveUser { get; set; }

    [JsonPropertyName("banned")] public bool IsBanned { get; set; }
}