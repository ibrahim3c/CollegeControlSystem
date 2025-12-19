using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Faculties.CreateFaculty
{
    public sealed record CreateFacultyCommand(
        string FullName,
        Guid DepartmentId,
        string AppUserId, // Link to existing Identity User
        string Degree
    ) : ICommand<Guid>;
}
