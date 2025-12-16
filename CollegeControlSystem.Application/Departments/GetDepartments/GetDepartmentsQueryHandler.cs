

using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;
using System.Collections.Generic;

namespace CollegeControlSystem.Application.Departments.GetDepartments
{
    internal sealed class GetDepartmentsQueryHandler : IQueryHandler<GetDepartmentsQuery, List<DepartmentResponse>>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public GetDepartmentsQueryHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<Result<List<DepartmentResponse>>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            // include programs when fetching departments
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);

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
