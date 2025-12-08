using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Courses
{
    public static class CourseErrors
    {
        public static readonly Error CodeEmpty = new("Course.CodeEmpty", "Course code cannot be empty.");
        public static readonly Error CodeInvalidFormat = new("Course.CodeInvalid", "Course code must follow format 'ABC 123' (3 Letters + 3 Digits).");
        public static readonly Error CreditsInvalid = new("Course.CreditsInvalid", "Credits must be greater than zero.");
        public static readonly Error PrerequisiteCycle = new("Course.PrerequisiteCycle", "A course cannot be a prerequisite of itself.");
        public static readonly Error PrerequisiteDuplicate = new("Course.PrerequisiteDuplicate", "This prerequisite is already added.");
        public static readonly Error InvalidHours = new("Course.InvalidHours","Lecture hours and lab hours must be zero or greater.");

    }
}
