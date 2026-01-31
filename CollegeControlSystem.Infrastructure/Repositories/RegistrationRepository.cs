using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class RegistrationRepository : IRegistrationRepository
    {
        private readonly AppDbContext _context;

        public RegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Registration>()
                .Include(r => r.Student) // Often needed for security checks (StudentId)
                .Include(r => r.CourseOffering) // Often needed for logic (ReleaseSeat)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Registration?> GetByStudentAndOfferingAsync(Guid studentId, Guid courseOfferingId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Registration>()
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseOfferingId == courseOfferingId, cancellationToken);
        }

        public async Task<List<Registration>> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
        {
            // This is tricky: Registration points to Offering, Offering points to Course.
            // We need to check history: "Did student take Course X in *any* semester?"
            return await _context.Set<Registration>()
                .Include(r => r.Grade) // Need Grade to check IsPassing
                .Include(r => r.CourseOffering)
                .Where(r => r.StudentId == studentId && r.CourseOffering.CourseId == courseId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Registration>> GetPendingByAdvisorIdAsync(Guid advisorId, CancellationToken cancellationToken = default)
        {
            // Join: Registration -> Student. Check if Student.AdvisorId matches.
            return await _context.Set<Registration>()
                .Include(r => r.Student)
                .Include(r => r.CourseOffering)
                    .ThenInclude(co => co.Course) // Needed for UI "Course Name"
                .Where(r => r.Status == RegistrationStatus.Pending)
                .Where(r => r.Student.AdvisorId == advisorId)
                .OrderBy(r => r.RegistrationDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Registration>> GetByStudentAndSemesterAsync(Guid studentId, string term, int year, CancellationToken cancellationToken = default)
        {
            // Parse string term back to Enum if needed, or if passed as string
            // Assuming 'term' parameter matches the Enum string stored in DB or Value Object logic

            return await _context.Set<Registration>()
                .Include(r => r.CourseOffering)
                    .ThenInclude(co => co.Course)
                .Include(r => r.CourseOffering)
                    .ThenInclude(co => co.Instructor)
                .Where(r => r.StudentId == studentId)
                .Where(r => r.CourseOffering.Semester.Year == year && r.CourseOffering.Semester.Term.ToString() == term)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Registration>> GetActiveByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            // "Active" usually means current schedule (Pending + Approved)
            // Usually filtered by "Current Semester" in real apps, but for "All Active" query:
            return await _context.Set<Registration>()
                .Include(r => r.CourseOffering)
                    .ThenInclude(co => co.Course)
                .Include(r => r.CourseOffering)
                    .ThenInclude(co => co.Instructor)
                .Where(r => r.StudentId == studentId)
                .Where(r => r.Status == RegistrationStatus.Pending || r.Status == RegistrationStatus.Approved)
                .ToListAsync(cancellationToken);
        }

        public void Add(Registration registration)
        {
            _context.Set<Registration>().Add(registration);
        }

        public void Update(Registration registration)
        {
            _context.Set<Registration>().Update(registration);
        }
    }
}
