using Prn232.Lab1.Service.Dtos;

namespace Prn232.Lab1.Service.Dtos.Courses;

public class CourseResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public SemesterSummaryResponse? Semester { get; set; }
    public List<EnrollmentSummaryResponse>? Enrollments { get; set; }
    public List<StudentSummaryResponse>? Students { get; set; }
}
