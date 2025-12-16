
using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.UpdateDepartment
{
    internal sealed class UpdateDepartmentCommandHandler : ICommandHandler<UpdateDepartmentCommand>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            if (department is null)
            {
                return Result.Failure(DepartmentErrors.NotFound);
            }

            var result=department.UpdateDetails(request.Name, request.Description);

            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
