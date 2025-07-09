using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Core.Models.Users;
using Core.Server.Database.Files;
using Core.Server.Database.GamePacks;
using Core.Server.Database.Lobbies;

namespace Core.Server.Database.Users;

public class User
{
    private static readonly Expression<Func<User, bool>> IsActiveExpression =
        user => !user.BannedAt.HasValue && user.ConfirmedEmail && user.ConfirmedByModerator;

    [Key] public long Id { get; init; }

    [Required, MinLength(4), MaxLength(255)]
    public string NickName { get; init; }

    [Required, EmailAddress] public string Email { get; init; }

    [Required, MinLength(4)] public string PasswordHash { get; init; }

    public ushort Rating { get; set; }

    public UserRole Role { get; init; }

    public bool ConfirmedEmail { get; init; }

    public bool ConfirmedByModerator { get; init; }

    public string? JwtToken { get; set; }

    public DateTime CreatedAt { get; init; }

    public DateTime? BannedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public Guid? ImageId { get; init; }

    public FileData? Image { get; init; }

    public ICollection<GamePack> GamePacks { get; init; }

    public ICollection<Lobby> HostedLobby { get; init; }

    public Guid? JoinedLobbyId { get; init; }

    public Lobby? JoinedLobby { get; init; }

    [NotMapped] public bool IsActive => IsActiveExpression.Compile()(this);

    [Obsolete(message: "Only for EF", error: true)]
    public User()
    {
    }

    public User(string nickName, string passwordHash, string email)
    {
        NickName = nickName;
        CreatedAt = DateTime.Now;
        PasswordHash = passwordHash;
        Email = email;
        Rating = 1000;
        Role = UserRole.Player;
    }

    public bool CheckAccess(UserRole minimumRole) => minimumRole switch
    {
        UserRole.Admin => Role == UserRole.Admin,
        UserRole.Moderator => Role is UserRole.Moderator or UserRole.Admin,
        _ => true,
    };
}