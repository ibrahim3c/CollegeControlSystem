using Bookify.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Departments
{
    public sealed class Program: Entity
    {
        private Program(Guid id,Name name, int requiredCredits, Guid departmentId):base(id)
        {
            Name = name;
            RequiredCredits = requiredCredits;
            DepartmentId = departmentId;
        }
        // for EF
        private Program()
        {
        }

        public Name Name { get; private set; }
        public int RequiredCredits { get; private set; }
        public Guid DepartmentId { get; private set; }

        public Department Department { get; private set; }

        public static Program Create(Guid id,Name name, int requiredCredits, Guid departmentId)
        {
            return new Program(id,name, requiredCredits, departmentId);
        }
    }
}
