using Prn232.Lab1.Service.Dtos.Enrollments;
using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Dtos.Students;

namespace Prn232.Lab1.Service.Dtos.Courses;

public class CourseResponseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public SemesterResponseDto? Semester { get; set; }
    public List<EnrollmentResponseDto>? Enrollments { get; set; }
    public List<StudentResponseDto>? Students { get; set; }
}
