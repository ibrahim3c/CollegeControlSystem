using CollegeControlSystem.Domain.Students;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Student?> GetByAcademicNumberAsync(string academicNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .FirstOrDefaultAsync(s => s.AcademicNumber == academicNumber, cancellationToken);
        }

        public async Task<List<Student>> GetByAdvisorIdAsync(Guid advisorId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program) // Often needed for the Advisor Dashboard
                .Where(s => s.AdvisorId == advisorId)
                .OrderBy(s => s.StudentName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            // "Active" typically means not Dismissed (and optionally not Graduated, depending on requirements).
           // Based on[cite: 197], Dismissed is a terminal status.
            return await _context.Set<Student>()
                .Where(s => s.AcademicStatus != AcademicStatus.Dismissed)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Set<Student>().AddAsync(student);
        }

        public void Update(Student student)
        {
            _context.Set<Student>().Update(student);
        }

        public async Task<Student> GetByIdWithProgramAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program)
                .Include(s => s.AppUser) // Helpful to get Email
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        }

        public async Task<Student> GetTranscriptDataAsync(Guid studentId, CancellationToken cancellationToken)
        {
            // Deep include to fetch the hierarchy: Student -> Registrations -> CourseOffering -> Course
            // This is necessary for generating the Transcript Report.
            return await _context.Set<Student>()
                .Include(s => s.Program) // Transcript Header info
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.CourseOffering)
                        .ThenInclude(co => co.Course) // Actual Course Details (Code, Credits)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Grade) // The Grades achieved
                .AsSplitQuery() // Optimization for deep includes to avoid Cartesian explosion
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        }
    }
}