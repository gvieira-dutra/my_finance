using System.ComponentModel.DataAnnotations;

namespace MyFinance.Core.Requests.Account;

public class LoginRequest : Request
{
    [Required(ErrorMessage = "Email")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Invalid password")]
    public string Password { get; set; } = string.Empty;
}
