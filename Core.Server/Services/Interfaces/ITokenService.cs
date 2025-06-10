using Core.Models.Auth;
using Core.Server.Database.Users;

namespace Core.Server.Services.Interfaces;

public interface ITokenService
{
    public string GenerateAccessToken(User userModel);

    public (string, DateTime) GenerateRefreshToken(User userModel, bool longUse);

    public User? Validate(string token);

    public TokenModel RefreshToken(TokenModel? tokenModel);
}