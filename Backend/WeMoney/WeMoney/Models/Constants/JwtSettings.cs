namespace WeMoney.Models.Constants;

public class JwtSettings
{
    public int JwtLifeRefreshToken { get; init; }
    public string SecretKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string Subject { get; init; } = null!;
}