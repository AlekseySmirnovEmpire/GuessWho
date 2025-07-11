using System.ComponentModel.DataAnnotations;

namespace Core.Models.Lobbies;

public class PasswordRequestBody
{
    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
    public string Password { get; set; }
}