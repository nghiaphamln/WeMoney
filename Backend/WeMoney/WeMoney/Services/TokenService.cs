using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WeMoney.Models.Constants;

namespace WeMoney.Services;

public class TokenService(IOptions<AppSettings> options)
{
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }

    public string GenerateRefreshToken()
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
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase)
           )
        {
            throw new SecurityTokenException("Token không hợp lệ");
        }
        
        return principal;
    }
}