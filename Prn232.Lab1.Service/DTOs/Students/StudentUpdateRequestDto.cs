using System.ComponentModel.DataAnnotations;
using Prn232.Lab1.Service.Validators;

namespace Prn232.Lab1.Service.Dtos.Students;

public class StudentUpdateRequestDto
{
    [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [FptuStudentEmail]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }
}
