using Bookify.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Departments
{
    public sealed class Department:Entity
    {
        private Department(Guid id, string name, string description):base(id)
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
        public string DepartmentName { get; private set; }
        public string? Description { get; private set; }

        public List<Program> Programs { get; private set; } = new();

        public static Department Create(Guid id, string name, string description)
        {
            return new Department(id, name, description);
        }

    }
}
