using System.ComponentModel.DataAnnotations;
using Prn232.Lab1.Service.Validators;

namespace Prn232.Lab1.Service.Dtos.Students;

public class StudentCreateRequestDto
{
    [Required(ErrorMessage = "FullName is required.")]
    [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [FptuStudentEmail]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "DateOfBirth is required.")]
    public DateTime DateOfBirth { get; set; }
}
