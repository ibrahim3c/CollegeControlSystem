namespace CollegeControlSystem.Domain.Faculties
{
    public interface IFacultyRepository
    {
        Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Useful for "Show me all professors in Computer Engineering"
        Task<List<Faculty>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);

        // Useful for assigning an advisor to a student
        Task<List<Faculty>> GetAdvisorsAsync(CancellationToken cancellationToken = default);

        void Add(Faculty faculty);
        void Update(Faculty faculty);
    }
}
