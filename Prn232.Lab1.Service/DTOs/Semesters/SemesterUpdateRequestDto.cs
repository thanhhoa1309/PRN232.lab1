namespace Prn232.Lab1.Service.Dtos.Semesters;

public class SemesterUpdateRequestDto
{
    public string? SemesterName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
