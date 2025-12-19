using FluentValidation;

namespace CollegeControlSystem.Application.Faculties.UpdateFacultyInfo
{
    public sealed class UpdateFacultyInfoCommandValidator : AbstractValidator<UpdateFacultyInfoCommand>
    {
        public UpdateFacultyInfoCommandValidator()
        {
            RuleFor(x => x.FacultyId)
                .NotEmpty()
                .WithMessage("FacultyId cannot be empty.");

            RuleFor(x => x.NewDegree)
                .NotEmpty()
                .WithMessage("Degree cannot be empty.")
                .MaximumLength(100)
                .WithMessage("Degree name must not exceed 100 characters");

        }
    }
}
