using System.ComponentModel.DataAnnotations;

namespace Prn232.Lab1.Service.Dtos.Auth;

public class UserRegistrationDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [RegularExpression("^(Admin|Staff|Lecturer|Student)$",
        ErrorMessage = "Role must be Admin, Staff, Lecturer, or Student.")]
    public string Role { get; set; } = string.Empty;
}
