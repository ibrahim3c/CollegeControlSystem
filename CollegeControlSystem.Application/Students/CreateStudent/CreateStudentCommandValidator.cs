using FluentValidation;
namespace CollegeControlSystem.Application.Students.CreateStudent
{
    public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.AcademicNumber).NotEmpty().MaximumLength(20);
            RuleFor(x => x.NationalId).NotEmpty().Length(14).Matches(@"^\d+$").WithMessage("National ID must be 14 digits.");
            RuleFor(x => x.ProgramId).NotEmpty();
            RuleFor(x => x.AppUserId).NotEmpty();
        }
    }
