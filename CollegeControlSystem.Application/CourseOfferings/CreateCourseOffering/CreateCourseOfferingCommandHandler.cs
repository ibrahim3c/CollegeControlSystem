using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Application.CourseOfferings.CreateCourseOffering
{
    internal sealed class CreateCourseOfferingCommandHandler : ICommandHandler<CreateCourseOfferingCommand, Guid>
    {
        private readonly ICourseOfferingRepository _offeringRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseOfferingCommandHandler(ICourseOfferingRepository offeringRepository, IUnitOfWork unitOfWork)
        {
            _offeringRepository = offeringRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCourseOfferingCommand request, CancellationToken cancellationToken)
        {
            // 1. Create Semester Value Object
            var semesterResult =  Semester.Create(request.Term, request.Year);
            if(semesterResult.IsFailure)
            {
                return Result<Guid>.Failure(semesterResult.Error);
            }   
            var semester = semesterResult.Value;

            // 2. Check for Duplicates (Optional but recommended)
            // Ideally, check if this Course + Semester + Instructor combination already exists to prevent double booking.

            // 3. Create Domain Entity
            var result = CourseOffering.Create(
                request.CourseId,
                request.InstructorId,
                semester,
                request.Capacity
            );

            if (result.IsFailure)
            {
                return Result<Guid>.Failure(result.Error);
            }

            // 4. Persist
            _offeringRepository.Add(result.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(result.Value.Id);
        }
    }
}
