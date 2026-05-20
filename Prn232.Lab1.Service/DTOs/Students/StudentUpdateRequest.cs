namespace Prn232.Lab1.Service.Dtos.Students;

public class StudentUpdateRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
