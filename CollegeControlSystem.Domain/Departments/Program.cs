using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Departments
{
    public sealed class Program: Entity
    {
        private Program(Guid id,string name, int requiredCredits, Guid departmentId):base(id)
        {
            Name = name;
            RequiredCredits = requiredCredits;
            DepartmentId = departmentId;
        }
        // for EF
        private Program()
        {
        }

        public string Name { get; private set; }
        public int RequiredCredits { get; private set; }
        public Guid DepartmentId { get; private set; }

        public Department Department { get; private set; }

        public static Program Create(Guid id,string name, int requiredCredits, Guid departmentId)
        {
            // validate inputs
            return new Program(id,name, requiredCredits, departmentId);
        }
    }
}
