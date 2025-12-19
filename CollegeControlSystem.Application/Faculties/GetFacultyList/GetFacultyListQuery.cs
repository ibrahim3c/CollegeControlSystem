using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Faculties.GetFacultyList
{
    public sealed record GetFacultyListQuery(
        Guid? DepartmentId = null
    ) : IQuery<List<GetFacultyListQueryResponse>>;


}
