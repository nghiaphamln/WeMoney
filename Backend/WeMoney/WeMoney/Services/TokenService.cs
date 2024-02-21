using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WeMoney.Models.Constants;
using WeMoney.Models.Entities;

namespace WeMoney.Services;

public class TokenService(IOptions<JwtSettings> jwtSettings)
{
    public (string Token, string RefreshToken) GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, jwtSettings.Value.Subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new(ClaimType.Id, user.Id!)
        };
        claims.AddRange(user.Role.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken();

        return (token, refreshToken);
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            jwtSettings.Value.Issuer,
            jwtSettings.Value.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: signinCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = jwtSettings.Value.Audience,
            ValidIssuer = jwtSettings.Value.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey))
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase)
           )
        {
            throw new SecurityTokenException("Token không hợp lệ");
        }
        return principal;
    }
}