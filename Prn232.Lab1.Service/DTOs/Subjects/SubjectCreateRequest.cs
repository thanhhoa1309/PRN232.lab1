namespace Prn232.Lab1.Service.Dtos.Subjects;

public class SubjectCreateRequest
{
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }
}
