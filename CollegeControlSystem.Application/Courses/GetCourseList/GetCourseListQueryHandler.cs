using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;
namespace CollegeControlSystem.Application.Courses.GetCourseList
{

    internal sealed class GetCourseListQueryHandler : IQueryHandler<GetCourseListQuery, List<GetCourseListQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCourseListQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<GetCourseListQueryResponse>>> Handle(GetCourseListQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.CourseRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);

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
