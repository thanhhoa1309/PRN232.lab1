namespace Prn232.Lab1.Service.Dtos.Enrollments;

public class EnrollmentCreateRequest
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime? EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
