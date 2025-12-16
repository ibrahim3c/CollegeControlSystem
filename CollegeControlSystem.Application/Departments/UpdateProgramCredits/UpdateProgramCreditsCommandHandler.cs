using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.UpdateProgramCredits
{
    internal sealed class UpdateProgramCreditsCommandHandler : ICommandHandler<UpdateProgramCreditsCommand>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProgramCreditsCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProgramCreditsCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            if (department is null)
            {
                return Result.Failure(DepartmentErrors.NotFound);
            }

            var result = department.UpdateProgramCredits(request.ProgramId, request.NewRequiredCredits);

            if (result.IsFailure)
            {
                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }

}
