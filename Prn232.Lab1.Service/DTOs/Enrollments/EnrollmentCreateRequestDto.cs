using System.ComponentModel.DataAnnotations;

namespace Prn232.Lab1.Service.Dtos.Enrollments;

public class EnrollmentCreateRequestDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "StudentId must be a positive integer.")]
    public int StudentId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CourseId must be a positive integer.")]
    public int CourseId { get; set; }

    public DateTime? EnrollDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [RegularExpression("^(Active|Completed|Dropped)$",
        ErrorMessage = "Status must be Active, Completed, or Dropped.")]
    public string Status { get; set; } = string.Empty;
}
