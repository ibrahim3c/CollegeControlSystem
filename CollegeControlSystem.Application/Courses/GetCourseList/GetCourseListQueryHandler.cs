using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;
namespace CollegeControlSystem.Application.Courses.GetCourseList
{

    internal sealed class GetCourseListQueryHandler : IQueryHandler<GetCourseListQuery, List<GetCourseListQueryResponse>>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseListQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Result<List<GetCourseListQueryResponse>>> Handle(GetCourseListQuery request, CancellationToken cancellationToken)
        {
            // Requires extending ICourseRepository with GetAllAsync or GetByDepartmentAsync
            // Assuming GetByDepartmentAsync(Guid? deptId) exists in Repo for this example
            var courses = await _courseRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);

            var response = courses.Select(c => new GetCourseListQueryResponse(
                c.Id,
                c.Code.Value, // Using the Value Object property
                c.Title,
                c.Credits
            )).ToList();

            return Result<List<GetCourseListQueryResponse>>.Success(response);
        }
    }
}
