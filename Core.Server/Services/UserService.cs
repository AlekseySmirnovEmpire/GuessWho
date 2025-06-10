using Core.Models.Auth;
using Core.Models.Users;
using Core.Server.Database.Users;
using Core.Server.Repositories;
using Microsoft.Extensions.Logging;

namespace Core.Server.Services;

public class UserService(ILogger<UserService> logger, IUserRepository repository)
{
    public User? FindByEmail(string email) =>
        string.IsNullOrEmpty(email) ? null : repository.Find(u => u.Email == email);

    public User? FindById(long id) => repository.Find(u => u.Id == id);

    public void SetRefreshToken(long userId, string token) =>
        repository.Update(u => u.Id == userId, sp => sp.SetProperty(u => u.JwtToken, token));

    public User CreateUser(SignUpModel? userModel)
    {
        if (userModel == null ||
            string.IsNullOrEmpty(userModel.NickName) ||
            string.IsNullOrEmpty(userModel.Email) ||
            string.IsNullOrEmpty(userModel.Password))
            throw new InvalidDataException("Поля не заполнены.");

        if (repository.Find(u => u.Email == userModel.Email && u.NickName == userModel.NickName) != null)
            throw new InvalidDataException("Пользователь с данным email или именем уже существует");

        try
        {
            var user = repository.Add(
                new User(
                    userModel.NickName,
                    PasswordService.GenerateHashPassword(userModel.Password),
                    userModel.Email));

            logger.LogInformation("Создан пользователь с ID: '{Id}'", user!.Id);

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при создании пользователя: {0}", ex.Message);

            throw new Exception("Не удалось создать пользователя!");
        }
    }

    public void DropRefreshToken(long userId) =>
        repository.Update(
            u => u.Id == userId,
            sp => sp.SetProperty(u => u.JwtToken, (string?)null));

    public bool ConfirmUserEmail(string? token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var claims = EmailVerificationService.VerifyLink(token);
        ArgumentNullException.ThrowIfNull(claims);

        var user = repository.Find(u => u.Id == claims.Id);
        ArgumentNullException.ThrowIfNull(user);

        if (user.ConfirmedEmail) return false;

        repository.Update(
            u => u.Id == user.Id,
            sp => sp.SetProperty(u => u.ConfirmedEmail, true));

        return true;
    }

    public bool ConfirmUserByModerator(long userId, User? moderator)
    {
        ArgumentNullException.ThrowIfNull(moderator);

        if (!moderator.CheckAccess(UserRole.Moderator)) throw new UnauthorizedAccessException();

        var user = repository.Find(u => u.Id == userId);
        ArgumentNullException.ThrowIfNull(user);

        if (user.ConfirmedByModerator) return false;

        repository.Update(
            u => u.Id == user.Id,
            sp => sp.SetProperty(u => u.ConfirmedByModerator, true));

        return true;
    }

    public bool ResetUserPassword(ResetPasswordModel? model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var claims = EmailVerificationService.VerifyLink(model.Token);
        ArgumentNullException.ThrowIfNull(claims);

        var user = repository.Find(u => u.Id == claims.Id);
        ArgumentNullException.ThrowIfNull(user);

        repository.Update(
            u => u.Id == user.Id,
            sp => sp.SetProperty(
                u => u.PasswordHash,
                PasswordService.GenerateHashPassword(model.Password)));

        return true;
    }
}