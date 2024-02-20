using System.ComponentModel.DataAnnotations;

namespace WeMoney.Models.Request.Auth;

[Serializable]
public class LoginRequest
{
    [Required(ErrorMessage = "Email không được bỏ trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
    public string Password { get; set; } = null!;
}