using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;

namespace CollegeControlSystem.Application.CourseOfferings.ChangeInstructor
{
    internal sealed class ChangeInstructorCommandHandler : ICommandHandler<ChangeInstructorCommand>
    {
        private readonly ICourseOfferingRepository _offeringRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeInstructorCommandHandler(ICourseOfferingRepository offeringRepository, IUnitOfWork unitOfWork)
        {
            _offeringRepository = offeringRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangeInstructorCommand request, CancellationToken cancellationToken)
        {
            var offering = await _offeringRepository.GetByIdAsync(request.OfferingId, cancellationToken);

            if (offering is null) return Result.Failure(CourseOfferingErrors.OfferingNotFound);

            var result = offering.ChangeInstructor(request.NewInstructorId);

            if (result.IsFailure) return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
