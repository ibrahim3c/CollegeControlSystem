using Bookify.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Registrations
{
    public sealed class Registration:Entity
    {
        public Registration() { }
        public Registration(Guid id, Guid studentId, Guid courseOfferingId, bool isRetake) : base(id)
        {
            StudentId = studentId;
            CourseOfferingId = courseOfferingId;
            Status = RegistrationStatus.Pending;
            IsRetake = isRetake;
        }

        public Guid StudentId { get; private set; }
        public Guid CourseOfferingId { get; private set; }
        public RegistrationStatus Status { get; private set; }

        // We store this to help the Grading Logic know if it needs to cap points
        public bool IsRetake { get; private set; }


        public void Approve() => Status = RegistrationStatus.Approved;

        public void Complete()
        {
            Status = RegistrationStatus.Completed;
        }

        public static class RegistrationFactory
        {
            public static Result<Registration> Create(Guid studentId, Guid courseOfferingId, bool isRetake)
            {
                if (studentId == Guid.Empty)
                    return Result<Registration>.Failure(Error.EmptyId("Student"));

                if (courseOfferingId == Guid.Empty)
                    return Result<Registration>.Failure(Error.EmptyId("CourseOffering"));

                var registration = new Registration(Guid.NewGuid(), studentId, courseOfferingId, isRetake);
                return Result<Registration>.Success(registration);
            }
        }

    }
}
