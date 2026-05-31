using System.ComponentModel.DataAnnotations;

namespace Prn232.Lab1.Repositories.Domain
{
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        [MaxLength(128)] public string? RefreshToken { get; set; }

    }
}
