using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings
{
    internal sealed class GetAvailableOfferingsQueryHandler : IQueryHandler<GetAvailableOfferingsQuery, List<OfferingQueryResponse>>
    {
        private readonly ICourseOfferingRepository _offeringRepository;

        public GetAvailableOfferingsQueryHandler(ICourseOfferingRepository offeringRepository)
        {
            _offeringRepository = offeringRepository;
        }

        public async Task<Result<List<OfferingQueryResponse>>> Handle(GetAvailableOfferingsQuery request, CancellationToken cancellationToken)
        {
            // 1. Create Value Object to query with
            var semesterResult = Semester.Create(request.Term, request.Year);
            if (semesterResult.IsFailure)
            {
                return Result<List<OfferingQueryResponse>>.Failure(semesterResult.Error);
            }
            var semester = semesterResult.Value;

            // 2. Fetch from Repo (Must Include Course & Instructor)
            var offerings = await _offeringRepository.GetBySemesterAsync(semester, cancellationToken);

            // 3. Map to DTO
            // Note: Instructor Name logic depends on how you load Faculty/User info.
            // Assuming your Repository Includes Faculty -> User

            var response = offerings.Select(o => new OfferingQueryResponse(
                o.Id,
                o.Course?.Code?.Value ?? "N/A",
                o.Course?.Title ?? "Unknown Title",
                o.Instructor.FacultyName, // Replace with actual name if Navigation Property exists
                o.Capacity,
                o.CurrentEnrolled,
                o.IsFull
            )).ToList();

            return Result<List<OfferingQueryResponse>>.Success(response);
        }
    }
}
