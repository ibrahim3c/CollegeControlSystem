using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetAllStudents
{
    internal sealed class GetAllStudentsQueryHandler : IQueryHandler<GetAllStudentsQuery, List<StudentListItemResponse>>
    {
        private readonly IStudentRepository _studentRepository;

        public GetAllStudentsQueryHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Result<List<StudentListItemResponse>>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetAllAsync(cancellationToken);

            var responses = students.Select(s => new StudentListItemResponse(
                s.Id,
                s.StudentName,
                s.AcademicNumber,
                s.Program?.Name ?? "Unassigned",
                s.CGPA,
                s.AcademicStatus.ToString(),
                s.AcademicLevel.ToString()
            )).ToList();

            return Result<List<StudentListItemResponse>>.Success(responses);
        }
    }
}
