
namespace CollegeControlSystem.Domain.Departments
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        //include Programs when fetching departments
        Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default);

        // Useful for validation "Does Department CCE exist?"
        Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        void Add(Department department);
        void Update(Department department);

        // populate Program with Department data for UI context
        Task<List<Program>> GetProgramsWithDepartmentAsync(CancellationToken cancellationToken = default);
        //Task DeleteAsync(Department department);
        // Delete typically restricted if students are linked
    }
}
