using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Domain.CourseOfferings
{
    public interface ICourseOfferingRepository
    {
        Task<CourseOffering?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Crucial for the "Available Courses" page
        Task<List<CourseOffering>> GetBySemesterAsync(Semester semester, CancellationToken cancellationToken = default);

        // Crucial for checking prerequisites (Did student pass Course X?)
        // Note: This logic might live in Student/Registration repo, but getting Offering by Course is useful.
        Task<List<CourseOffering>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

        void Add(CourseOffering offering);
        void Update(CourseOffering offering);
        Task<IEnumerable<CourseOffering>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken);
        // Delete is usually rarely used; usually we 'Archive' or 'Cancel' via status

        // check for duplicates (same course, same semester, same instructor) - optional but recommended to prevent double booking. This could be a method like:
        Task<CourseOffering> GetByCourseIdAsync(Guid courseId, Semester semester, Guid instructorId, CancellationToken cancellationToken = default);
    }
}
