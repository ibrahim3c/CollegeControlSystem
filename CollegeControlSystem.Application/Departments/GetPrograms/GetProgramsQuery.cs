using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Departments.GetPrograms
{
    public sealed record GetProgramsQuery() : IQuery<List<ProgramListResponse>>;
}
