

using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Application.Departments.GetDepartments
{
    internal sealed class GetDepartmentsQueryHandler : IQueryHandler<GetDepartmentsQuery, List<DepartmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<DepartmentResponse>>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            // include programs when fetching departments
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync(cancellationToken);

            var response = departments.Select(dept => new DepartmentResponse(
                dept.Id,
                dept.DepartmentName,
                dept.Description,
                dept.Programs.Select(prog => new ProgramResponse(
                    prog.Id,
                    prog.Name,
                    prog.RequiredCredits
                )).ToList()
            )).ToList();

            return Result<List<DepartmentResponse >>.Success(response);
        }
    }
}
