using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Shared;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class CourseOfferingRepository : ICourseOfferingRepository
    {
        private readonly AppDbContext _context;

        public CourseOfferingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CourseOffering?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // We generally need Course and Instructor details when fetching a single offering
            return await _context.Set<CourseOffering>()
                .Include(co => co.Course)
                .Include(co => co.Instructor)
                .FirstOrDefaultAsync(co => co.Id == id, cancellationToken);
        }

        public async Task<List<CourseOffering>> GetBySemesterAsync(Semester semester, CancellationToken cancellationToken = default)
        {
            // Used for "Available Courses" page
            return await _context.Set<CourseOffering>()
                .Include(co => co.Course)           // Include Title, Credits
                .Include(co => co.Instructor)       // Include Instructor Name
                                                    // Querying Value Object properties
                .Where(co => co.Semester.Year == semester.Year && co.Semester.Term == semester.Term)
                .OrderBy(co => co.Course.Code.Value)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CourseOffering>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<CourseOffering>()
                .Where(co => co.CourseId == courseId)
                .OrderByDescending(co => co.Semester.Year)
                .ThenByDescending(co => co.Semester.Term)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CourseOffering>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken)
        {
            return await _context.Set<CourseOffering>()
                .Include(co => co.Course)
                .Where(co => co.InstructorId == instructorId)
                .OrderByDescending(co => co.Semester.Year)
                .ThenByDescending(co => co.Semester.Term)
                .ToListAsync(cancellationToken);
        }

        public void Add(CourseOffering offering)
        {
            _context.Set<CourseOffering>().Add(offering);
        }

        public void Update(CourseOffering offering)
        {
            _context.Set<CourseOffering>().Update(offering);
        }
    }
}
