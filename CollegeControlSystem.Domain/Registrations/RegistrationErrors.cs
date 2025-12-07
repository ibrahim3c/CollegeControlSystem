using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Registrations
{
    public  static class RegistrationErrors
    {
        public static readonly Error AlreadyCompleted = new(
        "Registration.AlreadyCompleted",
        "Cannot modify a registration that is already completed.");

        public static readonly Error NotPending = new(
            "Registration.NotPending",
            "Only pending registrations can be approved or rejected.");

        public static readonly Error AlreadyDropped = new(
            "Registration.AlreadyDropped",
            "This registration has already been dropped.");
    }
}
