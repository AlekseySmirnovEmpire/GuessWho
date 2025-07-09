using System.ComponentModel.DataAnnotations;
using Core.Server.Database.Files;
using Core.Server.Database.Lobbies;
using Core.Server.Database.Users;

namespace Core.Server.Database.GamePacks;

public class GamePack
{
    [Key] public long Id { get; init; }

    [Required] public string DisplayName { get; init; }

    public string? Description { get; init; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; init; }

    public long UserCreatedId { get; init; }

    public User UserCreated { get; set; }

    public Guid FileId { get; init; }

    public FileData File { get; init; }

    public ICollection<Lobby> Lobbies { get; init; }

    [Obsolete("EF ONLY", error: true)]
    public GamePack()
    {
    }

    public GamePack(string displayName, long userCreatedId, Guid fileId, string? description = null)
    {
        DisplayName = displayName;
        Description = description;
        UserCreatedId = userCreatedId;
        FileId = fileId;
        CreatedAt = DateTime.Now;
    }
}