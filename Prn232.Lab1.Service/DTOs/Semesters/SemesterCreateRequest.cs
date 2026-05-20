namespace Prn232.Lab1.Service.Dtos.Semesters;

public class SemesterCreateRequest
{
    public string SemesterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
