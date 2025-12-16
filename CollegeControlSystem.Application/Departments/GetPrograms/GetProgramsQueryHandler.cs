using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.GetPrograms
{
    internal sealed class GetProgramsQueryHandler : IQueryHandler<GetProgramsQuery, List<ProgramListResponse>>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public GetProgramsQueryHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<Result<List<ProgramListResponse>>> Handle(GetProgramsQuery request, CancellationToken cancellationToken)
        {
            var programs = (await _departmentRepository.GetProgramsWithDepartmentAsync(cancellationToken)).Select(prog => new ProgramListResponse(
                    prog.Id,
                    prog.Name,
                    prog.Department.DepartmentName,
                    prog.RequiredCredits
                ))
                .OrderBy(p => p.Name)
                .ToList(); ;

            return Result<List<ProgramListResponse>>.Success(programs);
        }
    }
}
