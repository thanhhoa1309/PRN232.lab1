using System.ComponentModel.DataAnnotations;

namespace Prn232.Lab1.Service.Dtos.Auth;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
