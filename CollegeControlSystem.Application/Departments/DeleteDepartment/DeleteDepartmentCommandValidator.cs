//using FluentValidation;
//namespace CollegeControlSystem.Application.Departments.DeleteDepartment
//{
//    internal class DeleteDepartmentCommandValidator:AbstractValidator<DeleteDepartmentCommand>
//    {
//        public DeleteDepartmentCommandValidator()
//        {
//            RuleFor(x => x.DepartmentId)
//                .NotEmpty().WithMessage("DepartmentId cannot be empty.")
//                .NotEqual(Guid.Empty).WithMessage("DepartmentId cannot be Guid.Empty.");
//        }
//    }
//}
