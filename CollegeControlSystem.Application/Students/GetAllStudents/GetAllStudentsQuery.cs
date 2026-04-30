using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Students.GetAllStudents
{
    public record GetAllStudentsQuery : IQuery<List<StudentListItemResponse>>;
}
