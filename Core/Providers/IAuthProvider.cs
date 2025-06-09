using Core.Database.Users;
using Core.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace Core.Providers;

public interface IAuthProvider
{
    public TokenModel Login(LoginModel? loginModel);

    public UserCreateResponseModel SignUp(SignUpModel? signUpModel);

    public void Logout(string? token);

    public TokenModel RefreshToken(TokenModel refreshToken);

    public User GetCurrentUser(HttpContext httpContext);
}