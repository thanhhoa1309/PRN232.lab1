using FluentValidation;
using Prn232.Lab1.Service.Dtos.Courses;

namespace Prn232.Lab1.Service.Validators;

public class CourseCreateRequestValidator : AbstractValidator<CourseCreateRequestDto>
{
    public CourseCreateRequestValidator()
    {
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("CourseName is required.")
            .MaximumLength(100).WithMessage("CourseName cannot exceed 100 characters.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be a positive integer.");
    }
}
