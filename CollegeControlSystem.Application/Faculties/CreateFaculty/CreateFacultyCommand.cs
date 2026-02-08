using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Faculties.CreateFaculty
{
    public sealed record CreateFacultyCommand(
    string UserName,
    string Email,
    string Password,
    string? PhoneNumber,
    string FullName,
    Guid DepartmentId,
    string Degree,
    bool IsAdvisor
    ) : ICommand<Guid>;

}
