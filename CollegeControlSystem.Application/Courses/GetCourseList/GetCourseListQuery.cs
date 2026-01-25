using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Courses.GetCourseList
{
    public sealed record GetCourseListQuery(Guid? DepartmentId = null) : IQuery<List<GetCourseListQueryResponse>>;

}
