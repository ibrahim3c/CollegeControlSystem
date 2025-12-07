namespace CollegeControlSystem.Domain.Students
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Used during registration to check duplicates
        Task<Student?> GetByAcademicNumberAsync(string academicNumber, CancellationToken cancellationToken = default);

        // Used by Advisor Dashboard
        Task<List<Student>> GetByAdvisorIdAsync(Guid advisorId, CancellationToken cancellationToken = default);

        // Used for Control Engine (Batch Processing)
        Task<List<Student>> GetAllActiveAsync(CancellationToken cancellationToken = default);

        void Add(Student student);
        void Update(Student student);
    }
}
