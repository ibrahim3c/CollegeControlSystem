
namespace CollegeControlSystem.Domain.Departments
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Used to populate dropdowns "Select Department"
        Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default);

        // Useful for validation "Does Department CCE exist?"
        Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        void Add(Department department);
        void Update(Department department);
        // Delete typically restricted if students are linked
    }
}
