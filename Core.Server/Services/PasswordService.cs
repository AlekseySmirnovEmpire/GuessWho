namespace Core.Server.Services;

public class PasswordService
{
    public static string GenerateHashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public static bool Validate(string password, string hashPassword) => 
        BCrypt.Net.BCrypt.Verify(password, hashPassword);
}