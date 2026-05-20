using Prn232.Lab1.Service.Dtos;

namespace Prn232.Lab1.Service.Dtos.Students;

public class StudentResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public List<EnrollmentSummaryResponse>? Enrollments { get; set; }
}
