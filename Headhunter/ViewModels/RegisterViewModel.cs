using System.ComponentModel.DataAnnotations;

namespace Headhunter.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username field is empty")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email field is empty")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Name field is empty")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Role field is empty")]
    public string Role { get; set; }

    [Required(ErrorMessage = "Contact phone field is empty")]
    public string ContactPhone { get; set; }

    public string? Avatar { get; set; }

    [Required(ErrorMessage = "Password field is empty")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords are not equal")]
    public string ConfirmPassword { get; set; }
}
