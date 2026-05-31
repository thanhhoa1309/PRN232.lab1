using System.ComponentModel.DataAnnotations;

namespace Prn232.Lab1.Service.Dtos.Subjects;

public class SubjectCreateRequestDto
{
    [Required(ErrorMessage = "SubjectCode is required.")]
    [StringLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "SubjectName is required.")]
    [StringLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    [Required]
    [Range(1, 10, ErrorMessage = "Credit must be between 1 and 10.")]
    public int Credit { get; set; }
}
