using FluentValidation;
namespace CollegeControlSystem.Application.Faculties.CreateFaculty
{
    internal class CreateFacultyCommandValidator:AbstractValidator<CreateFacultyCommand>
    {
        public CreateFacultyCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required.");
            RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("App User ID is required.");
            RuleFor(x => x.Degree)
                .NotEmpty().WithMessage("Degree is required.")
                .MaximumLength(50).WithMessage("Degree cannot exceed 50 characters.");
        }
    }
}
