using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Application.Departments.GetPrograms
{
    internal sealed class GetProgramsQueryHandler : IQueryHandler<GetProgramsQuery, List<ProgramListResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProgramsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ProgramListResponse>>> Handle(GetProgramsQuery request, CancellationToken cancellationToken)
        {
            var programs = (await _unitOfWork.DepartmentRepository.GetProgramsWithDepartmentAsync(cancellationToken)).Select(prog => new ProgramListResponse(
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
