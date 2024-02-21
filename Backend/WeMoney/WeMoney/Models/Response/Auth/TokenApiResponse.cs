namespace WeMoney.Models.Response.Auth;

[Serializable]
public class TokenApiResponse
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}