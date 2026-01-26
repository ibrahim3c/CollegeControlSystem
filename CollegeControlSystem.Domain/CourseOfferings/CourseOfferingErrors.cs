using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.CourseOfferings
{
    public static class CourseOfferingErrors
    {
        public static readonly Error CapacityExceeded = new(
            "CourseOffering.CapacityExceeded",
            "Cannot enroll student. The course has reached its maximum capacity.");

        public static readonly Error InvalidCapacity = new(
            "CourseOffering.InvalidCapacity",
            "Capacity must be greater than zero.");

        public static readonly Error InstructorRequired = new(
            "CourseOffering.InstructorRequired",
            "A valid instructor must be assigned to the offering.");

        public static readonly Error CannotReduceCapacity = new(
            "CourseOffering.CapacityConflict",
            "Cannot reduce capacity below the number of currently enrolled students.");

        public static readonly Error OfferingNotFound = new(
            "CourseOffering.NotFound",
            "Course offering not found.");
    }
}
