using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Domain.Students;
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

        public async Task AddProgramAsync(Program program, CancellationToken cancellationToken = default)
        {
            await _context.Set<Program>().AddAsync(program, cancellationToken);
        }

        public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Department>()
                .Include(d => d.Programs)
                .FirstOrDefaultAsync(d => d.DepartmentName == name, cancellationToken);
        }

        public void Update(Department department)
        {
            _context.Set<Department>().Update(department);
        }

        public void Delete(Department department)
        {
            _context.Set<Department>().Remove(department);
        }

        public async Task<List<Program>> GetProgramsWithDepartmentAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Program>()
                .Include(p => p.Department)
                .OrderBy(p => p.Department.DepartmentName)
                .ThenBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Program?> GetProgramByIdAsync(Guid programId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Program>()
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.Id == programId, cancellationToken);
        }

        public async Task<bool> HasStudentsAsync(Guid programId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .AnyAsync(s => s.ProgramId == programId, cancellationToken);
        }

        public void RemoveProgram(Program program)
        {
            _context.Set<Program>().Remove(program);
        }
    }
}
