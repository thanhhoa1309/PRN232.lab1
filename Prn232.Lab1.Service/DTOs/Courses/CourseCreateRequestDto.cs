using System.ComponentModel.DataAnnotations;

namespace Prn232.Lab1.Service.Dtos.Courses;

public class CourseCreateRequestDto
{
    [Required(ErrorMessage = "CourseName is required.")]
    [StringLength(100, ErrorMessage = "CourseName cannot exceed 100 characters.")]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be a positive integer.")]
    public int SemesterId { get; set; }
}
