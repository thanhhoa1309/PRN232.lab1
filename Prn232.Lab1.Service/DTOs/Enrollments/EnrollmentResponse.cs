using Prn232.Lab1.Service.Dtos;

namespace Prn232.Lab1.Service.Dtos.Enrollments;

public class EnrollmentResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public StudentSummaryResponse? Student { get; set; }
    public CourseSummaryResponse? Course { get; set; }
}
