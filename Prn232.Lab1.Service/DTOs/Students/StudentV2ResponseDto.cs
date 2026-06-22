namespace Prn232.Lab1.Service.Dtos.Students;

public class StudentV2ResponseDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string ApiVersion { get; set; } = "2.0";
}
