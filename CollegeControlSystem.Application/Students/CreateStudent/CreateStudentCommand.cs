using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Students.CreateStudent
{
    public sealed record CreateStudentCommand(
        string FullName,
        string AcademicNumber,
        string NationalId,
        Guid ProgramId,
        string AppUserId
    ) : ICommand<Guid>;
}
