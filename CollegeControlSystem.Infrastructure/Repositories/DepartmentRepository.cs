using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Department>()
                .Include(d => d.Programs) // Explicitly include children
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Department>()
                .Include(d => d.Programs)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync(cancellationToken);
        }

        public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            // Note: The Domain Entity 'Department' currently uses 'DepartmentName' but not a short 'Code' property.
            // Mapping this requirement to Name for now.
            return await _context.Set<Department>()
                .FirstOrDefaultAsync(d => d.DepartmentName == code, cancellationToken);
        }

        public void Add(Department department)
        {
            _context.Set<Department>().Add(department);
        }

        public void Update(Department department)
        {
            _context.Set<Department>().Update(department);
        }

        public async Task<List<Program>> GetProgramsWithDepartmentAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Program>()
                .Include(p => p.Department)
                .OrderBy(p => p.Department.DepartmentName)
                .ThenBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
