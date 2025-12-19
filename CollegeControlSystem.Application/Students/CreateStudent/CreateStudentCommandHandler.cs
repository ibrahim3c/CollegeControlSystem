using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.CreateStudent
{
    internal sealed class CreateStudentCommandHandler : ICommandHandler<CreateStudentCommand, Guid>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            //Check for duplicate Academic Number
            var existingStudent = await _studentRepository.GetByAcademicNumberAsync(request.AcademicNumber, cancellationToken);
            if (existingStudent is not null)
            {
                return Result<Guid>.Failure(StudentErrors.DuplicateAcademicNumber);
            }

            var result = Student.Create(
                request.FullName,
                request.AcademicNumber,
                request.ProgramId,
                request.AppUserId,
                request.NationalId);

            if (result.IsFailure)
            {
                return Result<Guid>.Failure(result.Error);
            }

            await _studentRepository.AddAsync(result.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(result.Value.Id);
        }
    }
}