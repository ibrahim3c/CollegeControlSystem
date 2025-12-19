using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;
using System.Collections.Generic;
namespace CollegeControlSystem.Application.Faculties.GetFacultyList
{
    internal sealed class GetFacultyListQueryHandler : IQueryHandler<GetFacultyListQuery, List<GetFacultyListQueryResponse>>
    {
        private readonly IFacultyRepository _facultyRepository;

        public GetFacultyListQueryHandler(IFacultyRepository facultyRepository)
        {
            _facultyRepository = facultyRepository;
        }

        public async Task<Result<List<GetFacultyListQueryResponse>>> Handle(GetFacultyListQuery request, CancellationToken cancellationToken)
        {
            // to get all faculty of department or all faculty if no department specified
            List<Faculty> facultyMembers;

            if (request.DepartmentId.HasValue)
            {
                facultyMembers = await _facultyRepository.GetByDepartmentIdAsync(request.DepartmentId.Value, cancellationToken);
            }
            else
            {
                facultyMembers = await _facultyRepository.GetAllAsync(cancellationToken);
            }

            var response = facultyMembers.Select(f => new GetFacultyListQueryResponse(
                f.Id,
                f.FacultyName,
                f.Degree,
                f.Department?.DepartmentName ?? "Unknown" // Ensure Repository includes Department
            )).ToList();

            return Result<List<GetFacultyListQueryResponse>>.Success(response);
        }
    }
    }
