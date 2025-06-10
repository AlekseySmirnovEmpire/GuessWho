namespace Core.Models.Users;

public class UserModel
{
    public long Id { get; set; }
    public string NickName { get; set; }
    public UserRole Role { get; set; }
    public int Rating { get; set; }
}