using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.GetFacultyById
{
    internal sealed class GetFacultyByIdQueryHandler : IQueryHandler<GetFacultyByIdQuery, GetFacultyByIdQueryResponse>
    {
        private readonly IFacultyRepository _facultyRepository;

        public GetFacultyByIdQueryHandler(IFacultyRepository facultyRepository)
        {
            _facultyRepository = facultyRepository;
        }

        public async Task<Result<GetFacultyByIdQueryResponse>> Handle(GetFacultyByIdQuery request, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetByIdAsync(request.Id, cancellationToken);

            if (faculty is null)
            {
                return Result<GetFacultyByIdQueryResponse>.Failure(FacultyErrors.NotFound);
            }

            var dto = new GetFacultyByIdQueryResponse(
                faculty.Id,
                faculty.FacultyName,
                faculty.Degree,
                faculty.Department?.DepartmentName ?? "Unassigned",
                faculty.AppUser?.Email ?? "No Email",
                faculty.AppUserId
            );

            return Result<GetFacultyByIdQueryResponse>.Success(dto);
        }
    }
}