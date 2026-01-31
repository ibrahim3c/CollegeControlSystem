using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Course>()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Course?> GetByIdWithPrerequisitesAsync(Guid id, CancellationToken ct = default)
        {
            // This is crucial for the "Get Course Details" query
            return await _context.Set<Course>()
                .Include(c => c.Prerequisites)
                    .ThenInclude(cp => cp.PrerequisiteCourse) // Load the actual Prereq Course Entity
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<bool> IsCodeUniqueAsync(string normalizedCode, CancellationToken cancellationToken = default)
        {
            // Because of the HasConversion, we need to compare apples to apples.
            // Option 1: Client evaluation (Slow)
            // Option 2: Construct the Value Object to compare against.

            var codeResult = CourseCode.Create(normalizedCode);
            if (codeResult.IsFailure) return false; // Invalid code format implies it doesn't exist in DB anyway

            return !await _context.Set<Course>()
                .AnyAsync(c => c.Code.Equals(codeResult.Value), cancellationToken);
        }

        public async Task<List<Course>> GetByDepartmentAsync(Guid? departmentId, CancellationToken cancellationToken)
        {
            var query = _context.Set<Course>().AsQueryable();

            if (departmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentId == departmentId.Value);
            }

            return await query
                .OrderBy(c => c.Code) // The Value Object usually implements logic to sort by string
                .ToListAsync(cancellationToken);
        }

        public void Add(Course course)
        {
            _context.Set<Course>().Add(course);
        }

        public void Update(Course course)
        {
            _context.Set<Course>().Update(course);
        }
    }
}
