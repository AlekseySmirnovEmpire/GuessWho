using Core.Database.Users;
using Core.Models.Auth;
using Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class UserService(ILogger<UserService> logger, IUserRepository repository)
{
    public User? FindByEmail(string email) =>
        string.IsNullOrEmpty(email) ? null : repository.Find(u => u.Email == email);

    public User? FindById(long id) => repository.Find(u => u.Id == id);

    public void SetRefreshToken(long userId, string token) =>
        repository.Update(u => u.Id == userId, sp => sp.SetProperty(u => u.JwtToken, token));

    public void CreateUser(SignUpModel? userModel)
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
}