using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Models.Users;

public class ChangeUserData
{
    [JsonPropertyName("nickName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NickName { get; init; }
}