using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.AddProgram
{
    internal sealed class AddProgramCommandHandler : ICommandHandler<AddProgramCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddProgramCommandHandler(
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddProgramCommand request, CancellationToken cancellationToken)
        {
            // 1. Load Aggregate Root
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

            if (department is null)
            {
                return Result<Guid>.Failure(DepartmentErrors.NotFound);
            }

            // 2. Execute Domain Logic (Add Program)
            var programResult = department.AddProgram(request.Name, request.RequiredCredits);

            if (programResult.IsFailure)
            {
                return Result<Guid>.Failure(programResult.Error);
            }

            // 3. Save Changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return the new Program ID
            return Result<Guid>.Success(programResult.Value.Id);
        }
    }
}
