using Prn232.Lab1.Service.Dtos.Courses;

namespace Prn232.Lab1.Service.Dtos.Semesters;

public class SemesterResponseDto
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CourseResponseDto>? Courses { get; set; }
}
