using System.ComponentModel.DataAnnotations;

namespace Prn232.Lab1.Service.Dtos.Users;

public class UserCreateRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Admin|Staff|Lecturer|Student)$",
        ErrorMessage = "Role must be Admin, Staff, Lecturer, or Student.")]
    public string Role { get; set; } = string.Empty;
}
