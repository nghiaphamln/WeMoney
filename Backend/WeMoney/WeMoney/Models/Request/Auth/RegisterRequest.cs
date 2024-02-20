using System.ComponentModel.DataAnnotations;

namespace WeMoney.Models.Request.Auth;

[Serializable]
public class RegisterRequest
{
    [Required(ErrorMessage = "Email không được bỏ trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
        ErrorMessage = "Mật khẩu tối thiểu 8 ký tự, bao gồm chữ hoa, thường, số và ký tự đặc biệt")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Tên không được bỏ trống")]
    [Length(5, 100, ErrorMessage = "Tên phải từ 5 ký tự")]
    public string FullName { get; set; } = null!;
}