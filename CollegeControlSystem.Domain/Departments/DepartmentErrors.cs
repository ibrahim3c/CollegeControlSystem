using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Departments
{
    public static class DepartmentErrors
    {
        public static readonly Error NameRequired = new(
            "Department.NameRequired",
            "Department name cannot be empty.");

        public static readonly Error InvalidCredits = new(
            "Program.InvalidCredits",
            "Required credits must be greater than zero.");

        public static readonly Error DepartmentRequired = new(
            "Program.DepartmentRequired",
            "A program must belong to a department.");

        public static readonly Error DuplicateProgram = new("Department.DuplicateProgram",
            "A program with this name already exists in the department.");

        public static readonly Error ProgramNotFound = new("Department.ProgramNotFound", "Program not found in this department.");
        public static readonly Error NotFound = new("Department.NotFound", "Department not found.");

        //public static readonly Error NoProgram = new("Department.NoProgram", "Department has no programs.");
        public static readonly Error HasPrograms = new("Department.HasPrograms", "Cannot delete department because it has associated programs.");



    }
}
