using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Departments.GetDepartments
{
    public sealed record GetDepartmentsQuery() : IQuery<List<DepartmentResponse>>;
}
