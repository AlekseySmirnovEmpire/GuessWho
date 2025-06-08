using Core.Database.Users;
using Core.Models.Auth;

namespace Core.Services.Interfaces;

public interface ITokenService
{
    public string GenerateAccessToken(User userModel);

    public (string, DateTime) GenerateRefreshToken(User userModel, bool longUse);

    public User? Validate(string token);

    public TokenModel RefreshToken(TokenModel? tokenModel);
}