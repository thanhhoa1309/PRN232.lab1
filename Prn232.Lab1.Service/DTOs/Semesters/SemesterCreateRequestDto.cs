using System.ComponentModel.DataAnnotations;

namespace Prn232.Lab1.Service.Dtos.Semesters;

public class SemesterCreateRequestDto
{
    [Required(ErrorMessage = "SemesterName is required.")]
    [StringLength(100, ErrorMessage = "SemesterName cannot exceed 100 characters.")]
    public string SemesterName { get; set; } = string.Empty;

    [Required(ErrorMessage = "StartDate is required.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "EndDate is required.")]
    public DateTime EndDate { get; set; }
}
