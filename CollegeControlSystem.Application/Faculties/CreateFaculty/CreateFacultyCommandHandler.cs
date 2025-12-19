using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.CreateFaculty
{
    internal sealed class CreateFacultyCommandHandler : ICommandHandler<CreateFacultyCommand, Guid>
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateFacultyCommandHandler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork)
        {
            _facultyRepository = facultyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateFacultyCommand request, CancellationToken cancellationToken)
        {
            var result = Faculty.Create(
                request.FullName,
                request.DepartmentId,
                request.AppUserId,
                request.Degree
            );

            if (result.IsFailure)
            {
                return Result<Guid>.Failure(result.Error);
            }

            var faculty = result.Value;

            await _facultyRepository.AddAsync(faculty);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(faculty.Id);
        }
    }
}
