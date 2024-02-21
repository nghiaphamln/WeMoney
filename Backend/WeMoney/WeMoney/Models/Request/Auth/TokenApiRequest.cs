using System.ComponentModel.DataAnnotations;

namespace WeMoney.Models.Request.Auth;

[Serializable]
public class TokenApiRequest
{
    [Required] public string Token { get; set; } = null!;
    [Required] public string RefreshToken { get; set; } = null!;
}