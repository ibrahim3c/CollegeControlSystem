namespace CollegeControlSystem.Domain.Courses
{
    public interface ICourseRepository
    {
        // Get course with parsed Code Value Object
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Check uniqueness of code "ABC 123"
        Task<bool> IsCodeUniqueAsync(string normalizedCode, CancellationToken cancellationToken = default);

        void Add(Course course);
        void Update(Course course);
    }
}
