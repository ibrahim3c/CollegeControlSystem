namespace CollegeControlSystem.Application.Faculties.GetFacultyById
{

    public sealed record GetFacultyByIdQueryResponse(
    Guid Id,
    string Name,
    string Degree,
    string DepartmentName,
    string Email,
    string AppUserId
);
}
