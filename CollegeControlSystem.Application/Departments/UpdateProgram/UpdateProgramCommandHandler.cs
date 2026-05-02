using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.UpdateProgram
{
    internal sealed class UpdateProgramCommandHandler : ICommandHandler<UpdateProgramCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProgramCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

            if (department is null)
            {
                return Result.Failure(DepartmentErrors.NotFound);
            }

            var result = department.UpdateProgramName(request.ProgramId, request.Name);

            if (result.IsFailure)
            {
                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
