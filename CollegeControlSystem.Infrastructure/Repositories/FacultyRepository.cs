using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class FacultyRepository : IFacultyRepository
    {
        private readonly AppDbContext _context;

        public FacultyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Faculty>()
                .Include(f => f.Department) // Include Dept details
                .Include(f => f.AppUser)    // Include User details (Email)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<List<Faculty>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<Faculty>()
                .Include(f => f.Department)
                .OrderBy(f => f.FacultyName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Faculty>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Faculty>()
                .Where(f => f.DepartmentId == departmentId)
                .Include(f => f.Department)
                .OrderBy(f => f.FacultyName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Faculty>> GetAdvisorsAsync(CancellationToken cancellationToken = default)
        {
            // NOTE: In many systems, "Advisor" is a Role. 
            // If *all* faculty can be advisors, this returns all.
            // If only specific faculty are advisors, you would need to filter here.
            // For now, assuming all Faculty members are eligible to be Advisors.

            return await _context.Set<Faculty>()
                .Include(f => f.Department)
                .OrderBy(f => f.FacultyName)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Faculty faculty)
        {
            await _context.Set<Faculty>().AddAsync(faculty);
        }

        public void Update(Faculty faculty)
        {
            _context.Set<Faculty>().Update(faculty);
        }
    }
}
