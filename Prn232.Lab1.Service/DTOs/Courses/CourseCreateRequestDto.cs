namespace Prn232.Lab1.Service.Dtos.Courses;

public class CourseCreateRequestDto
{
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
}
