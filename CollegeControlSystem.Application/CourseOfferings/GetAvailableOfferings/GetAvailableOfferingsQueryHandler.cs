using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings
{
    internal sealed class GetAvailableOfferingsQueryHandler : IQueryHandler<GetAvailableOfferingsQuery, List<OfferingQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetAvailableOfferingsQueryHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<List<OfferingQueryResponse>>> Handle(GetAvailableOfferingsQuery request, CancellationToken cancellationToken)
        {
            Semester? semester = null;

            // 1. Conditionally Create Semester Value Object if both Term and Year are provided
            if (!string.IsNullOrWhiteSpace(request.Term) && request.Year.HasValue)
            {
                var semesterResult = Semester.Create(request.Term, request.Year.Value);
                if (semesterResult.IsFailure)
                {
                    return Result<List<OfferingQueryResponse>>.Failure(semesterResult.Error);
                }
                semester = semesterResult.Value;
            }

            // 2. Fetch from Repo using the flexible query method
            var offerings = await unitOfWork.CourseOfferingRepository.GetAvailableOfferingsAsync(
                semester,
                request.CourseId,
                request.InstructorId,
                cancellationToken);

            // 3. Map to DTO
            var response = offerings.Select(o => new OfferingQueryResponse(
                o.Id,
                o.Course?.Code?.Value ?? "N/A",
                o.Course?.Title ?? "Unknown Title",
                o.Instructor?.FacultyName ?? "Unknown Instructor",
                o.Capacity,
                o.CurrentEnrolled,
                o.IsFull
            )).ToList();

            return Result<List<OfferingQueryResponse>>.Success(response);
        }
    }
}
