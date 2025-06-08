using Core.Database.Email;
using Core.Models.Auth;
using Core.Providers;
using Core.Services;
using Core.Services.Interfaces;

namespace Infrastructure.Providers.Auth;

public class JwtAuthProvider(
    UserService userService, 
    ITokenService tokenService, 
    IEmailService emailService, 
    IEmailTemplatingService emailTemplatingService) 
    : IAuthProvider
{
    public TokenModel Login(LoginModel? loginModel)
    {
        if (loginModel == null || string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
            throw new Exception("Заполните данные!");

        var user = userService.FindByEmail(loginModel.Email);
        if (user == null) throw new Exception("Неверно введены данные!");

        if (!PasswordService.Validate(loginModel.Password, user.PasswordHash)) 
            throw new Exception("Неверно введён пароль!");

        if (!user.IsActive) throw new Exception("Ваш аккаунт временно деактивирован или не был подтверждён!");

        var accessToken = tokenService.GenerateAccessToken(user);
        var (refreshToken, _) = tokenService.GenerateRefreshToken(user, loginModel.RememberMe);

        userService.SetRefreshToken(user.Id, refreshToken);

        return new TokenModel
        {
            Access = accessToken,
            Refresh = refreshToken
        };
    }

    public UserCreateResponseModel SignUp(SignUpModel? signUpModel)
    {
        var user = userService.CreateUser(signUpModel);

        emailService.PutNewMessageInQueue(new EmailSendingQueue(
            "Подтверждение регистрации",
            emailTemplatingService.GenerateEmailTemplate(SubstitutionEmailTemplates.ConfirmEmail, user),
            user.Email,
            EmailPriority.Medium));

        return new UserCreateResponseModel();
    }

    public void Logout(string? token)
    {
        if (string.IsNullOrEmpty(token))
            throw new Exception("Некорректный токен!");

        var user = tokenService.Validate(token);
        if (user == null)
            throw new Exception("Некорректный токен!");

        userService.DropRefreshToken(user.Id);
    }

    public TokenModel RefreshToken(TokenModel? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken?.Refresh))
            throw new UnauthorizedAccessException("Некорректный токен!");

        var model = tokenService.RefreshToken(refreshToken);
        if (model == null)
            throw new UnauthorizedAccessException("Некорректный токен!");

        return model;
    }
}