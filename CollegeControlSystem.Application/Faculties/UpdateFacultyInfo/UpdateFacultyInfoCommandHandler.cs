using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;
namespace CollegeControlSystem.Application.Faculties.UpdateFacultyInfo
{
    internal sealed class UpdateFacultyInfoCommandHandler : ICommandHandler<UpdateFacultyInfoCommand>
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFacultyInfoCommandHandler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork)
        {
            _facultyRepository = facultyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateFacultyInfoCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetByIdAsync(request.FacultyId, cancellationToken);

            if (faculty is null)
            {
                return Result.Failure(FacultyErrors.NotFound);
            }

            var result = faculty.UpdateDegree(request.NewDegree);


            if (result.IsFailure) return result;

            _facultyRepository.Update(faculty);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
