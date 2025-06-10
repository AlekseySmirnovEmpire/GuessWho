using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Core.Models.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    [Description("Администратор")] Admin = 1,

    [Description("Модератор")] Moderator = 2,

    [Description("Игрок")] Player = 3,
}