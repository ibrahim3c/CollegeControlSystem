using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Faculties.UpdateFacultyInfo
{
    public sealed record UpdateFacultyInfoCommand(
        Guid FacultyId,
        string NewDegree
    ) : ICommand;
}
