namespace Prn232.Lab1.Service.Dtos.Subjects;

public class SubjectCreateRequestDto
{
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }
}
