using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Core.Database.Users;
using Core.Models.Auth;
using Core.Models.Users;
using Core.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Core.Services;

public class JwtTokenService(UserService userService) : ITokenService
{
    private const int AccessTokenExpireSeconds = 15;
    private const int RefreshTokenExpireDays = 1;
    private const int RefreshTokenExpireMonths = 12;

    public string GenerateAccessToken(User userModel) =>
        new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("SERVER_URL"),
            audience: Environment.GetEnvironmentVariable("SERVER_URL"),
            claims: new List<Claim>(),
            expires: DateTime.Now.AddSeconds(AccessTokenExpireSeconds),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_ACCESS_SECRET") ?? string.Empty)),
                SecurityAlgorithms.HmacSha256))
        {
            Payload =
            {
                ["user"] = JsonSerializer.Serialize(new UserClaims(userModel))
            }
        });

    public (string, DateTime) GenerateRefreshToken(User userModel, bool longUse)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JWT_REFRESH_SECRET") ?? string.Empty));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var expires = longUse
            ? DateTime.Now.AddDays(RefreshTokenExpireDays)
            : DateTime.Now.AddMonths(RefreshTokenExpireMonths);

        var tokenOptions = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("SERVER_URL"),
            audience: Environment.GetEnvironmentVariable("SERVER_URL"),
            claims: new List<Claim>(),
            expires: expires,
            signingCredentials: signInCredentials)
        {
            Payload =
            {
                ["user"] = JsonSerializer.Serialize(new UserClaims(userModel))
            }
        };

        return (new JwtSecurityTokenHandler().WriteToken(tokenOptions), expires);
    }

    public User? Validate(string token)
    {
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            Environment.GetEnvironmentVariable("JWT_ACCESS_SECRET") ?? string.Empty)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                },
                out var validToken);

            if (validToken is not JwtSecurityToken jwtToken)
            {
                return null;
            }

            if (!jwtToken.Payload.TryGetValue("user", out var user) || user == null)
            {
                return null;
            }

            var tokenUserModel = JsonSerializer.Deserialize<UserClaims>(user.ToString() ?? string.Empty);
            if (tokenUserModel == null) return null;

            var userModel = userService.FindById(tokenUserModel.Id);
            return userModel is not { IsActive: true }
                ? null
                : userModel;
        }
        catch
        {
            return null;
        }
    }

    public TokenModel RefreshToken(TokenModel? tokenModel)
    {
        if (tokenModel == null || string.IsNullOrEmpty(tokenModel.Refresh))
            throw new InvalidDataException("Остуствует тело запроса!");

        var user = ValidateRefresh(tokenModel.Refresh);

        if (user == null)
            throw new UnauthorizedAccessException("Рефреш токен некорректный!");

        var accessToken = GenerateAccessToken(user);

        return new TokenModel
        {
            Access = accessToken,
            Refresh = tokenModel.Refresh
        };
    }

    private User? ValidateRefresh(string token)
    {
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            Environment.GetEnvironmentVariable("JWT_REFRESH_SECRET") ?? string.Empty)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                },
                out var validToken);

            if (validToken is not JwtSecurityToken jwtToken) return null;

            if (!jwtToken.Payload.TryGetValue("user", out var user) || user == null) return null;

            var tokenUserModel = JsonSerializer.Deserialize<UserClaims>(user.ToString() ?? string.Empty);
            if (tokenUserModel == null) return null;

            var userModel = userService.FindById(tokenUserModel.Id);
            return userModel is not { IsActive: true }
                ? null
                : userModel;
        }
        catch
        {
            return null;
        }
    }
}