using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings
{
    //public sealed record GetAvailableOfferingsQuery(string Term, int Year) : IQuery<List<OfferingQueryResponse>>;
    public sealed record GetAvailableOfferingsQuery(
        string? Term,
        int? Year,
        Guid? CourseId,
        Guid? InstructorId
    ) : IQuery<List<OfferingQueryResponse>>;
}
