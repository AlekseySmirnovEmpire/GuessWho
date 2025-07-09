using System.ComponentModel.DataAnnotations;
using Core.Models.Lobbies;
using Core.Server.Database.Files;
using Core.Server.Database.GamePacks;
using Core.Server.Database.Users;

namespace Core.Server.Database.Lobbies;

public class Lobby
{
    [Key] public Guid Id { get; init; }

    [Required] public string DisplayName { get; init; }

    public long HostId { get; init; }

    public User Host { get; init; }

    public ushort PlayersCount { get; init; }

    public ushort RatingChange { get; init; }

    public DateTime CreatedAt { get; init; }

    public string? Password { get; init; }

    public long GamePackId { get; set; }

    public GamePack GamePack { get; set; }

    public List<LobbyCondition> Conditions { get; init; } = [];

    public ICollection<User> Users { get; set; }

    [Obsolete("EF ONLY", error: true)]
    public Lobby()
    {
    }

    public Lobby(string displayName, long hostId, ushort playersCount, ushort ratingChange, long gamePackId,
        string? password = null, List<LobbyCondition>? conditions = null)
    {
        Id = Guid.NewGuid();
        DisplayName = displayName;
        HostId = hostId;
        PlayersCount = playersCount;
        RatingChange = ratingChange;
        CreatedAt = DateTime.Now;
        GamePackId = gamePackId;
        Password = password;
        Conditions = conditions ?? [];
    }
}