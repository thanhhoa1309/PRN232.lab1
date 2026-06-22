using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Prn232.Lab1.Service.Validators;

/// <summary>
/// Mã môn FPTU: 2 chữ cái (SE, CE, AI, ...) + 5 chữ số, ví dụ SE19886, CE18793.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public partial class FptSubjectCodeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string code || string.IsNullOrWhiteSpace(code))
            return ValidationResult.Success;

        return FptSubjectCodeRegex().IsMatch(code)
            ? ValidationResult.Success
            : new ValidationResult("Subject code must follow FPTU format (e.g. SE19886, CE18793).");
    }

    [GeneratedRegex(@"^(SE|CE|AI|SS|DB|PR|MA|EN)[0-9]{5}$", RegexOptions.IgnoreCase)]
    private static partial Regex FptSubjectCodeRegex();
}
