using Bookify.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Departments
{
    public sealed class Department:Entity
    {
        private Department(Guid id, Name name, Description description):base(id)
        {
            Id = id;
            DepartmentName = name;
            Description = description;
        }

        // for EF
        private Department()
        {
        }

        public Guid Id { get; private set; }
        public Name DepartmentName { get; private set; }
        public Description? Description { get; private set; }

        public List<Program> Programs { get; private set; } = new();

        public static Department Create(Guid id, Name name, Description description)
        {
            return new Department(id, name, description);
        }

    }
}
