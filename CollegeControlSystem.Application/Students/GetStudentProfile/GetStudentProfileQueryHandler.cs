using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetStudentProfile
{
    internal sealed class GetStudentProfileQueryHandler : IQueryHandler<GetStudentProfileQuery, StudentResponse>
    {
        private readonly IStudentRepository _studentRepository;

        public GetStudentProfileQueryHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Result<StudentResponse>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdWithProgramAsync(request.StudentId, cancellationToken);

            if (student is null) return Result<StudentResponse>.Failure(StudentErrors.StudentNotFound);

            // Use your Domain Logic: Article 12 Load Limits
            int maxCredits = student.GetMaxAllowedCreditHours();

            var response = new StudentResponse(
                student.Id,
                student.StudentName,
                student.AcademicNumber,
                student.program.Name,
                student.CGPA,
                student.AcademicStatus.ToString(),
                student.AcademicLevel.ToString(),
                maxCredits
            );

            return Result<StudentResponse>.Success(response);
        }
    }
}
