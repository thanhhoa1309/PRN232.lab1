using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Dtos.Students;

namespace Prn232.Lab1.Service.Dtos.Enrollments;

public class EnrollmentResponseDto
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public StudentResponseDto? Student { get; set; }
    public CourseResponseDto? Course { get; set; }
}
