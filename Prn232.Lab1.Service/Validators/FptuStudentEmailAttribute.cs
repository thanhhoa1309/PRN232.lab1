using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Prn232.Lab1.Service.Validators;

/// <summary>
/// Validates FPTU student email format: SE19886@fpt.edu.vn, CE18793@fpt.edu.vn
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FptuStudentEmailAttribute : ValidationAttribute
{
    private static readonly Regex Pattern =
        new(@"^[A-Z]{2}\d{5}@fpt\.edu\.vn$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is null || (value is string s && string.IsNullOrWhiteSpace(s)))
            return ValidationResult.Success;

        if (value is not string email)
            return new ValidationResult("Email must be a string.");

        return Pattern.IsMatch(email)
            ? ValidationResult.Success
            : new ValidationResult("Email must follow FPTU format: SE19886@fpt.edu.vn");
    }
}
