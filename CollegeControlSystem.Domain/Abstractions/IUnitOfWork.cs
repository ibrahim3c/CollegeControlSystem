using CollegeControlSystem.Domain.CourseOffering;
using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Domain.Abstractions;

public interface IUnitOfWork
{
    IStudentRepository StudentRepository { get; }
    IFacultyRepository FacultieRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    ICourseOfferingRepository CourseOfferingRepository{ get; }
    ICourseRepository CourseRepository{ get; }
    IGradeRepository GradeRepository{ get; }
    IRegistrationRepository RegistrationRepository{ get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}