using Core.Models.Users;
using Core.Server.Database.Users;

namespace Core.Server.Models;

public class UserClaims
{
    public long Id { get; set; }
    public string NickName { get; set; }
    public UserRole Role { get; set; }
    public int Rating { get; set; }

    [Obsolete(message: "Only for JSON", error: true)]
    public UserClaims()
    {
    }

    public UserClaims(User user)
    {
        Id = user.Id;
        NickName = user.NickName;
        Role = user.Role;
        Rating = user.Rating;
    }
}