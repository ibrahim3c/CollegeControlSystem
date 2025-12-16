//using CollegeControlSystem.Application.Abstractions.Messaging;
//using CollegeControlSystem.Domain.Abstractions;
//using CollegeControlSystem.Domain.Departments;

//namespace CollegeControlSystem.Application.Departments.DeleteDepartment
//{
//    internal sealed class DeleteDepartmentCommandHandler : ICommandHandler<DeleteDepartmentCommand>
//    {
//        private readonly IDepartmentRepository _departmentRepository;
//        private readonly IUnitOfWork _unitOfWork;

//        public DeleteDepartmentCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
//        {
//            _departmentRepository = departmentRepository;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
//        {
//            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
//            if (department is null)
//            {
//                return Result.Failure(DepartmentErrors.NotFound);
//            }

//            // Integrity Check: Cannot delete if it has programs
//            if (department.Programs.Any())
//            {
//                return Result.Failure(DepartmentErrors.HasPrograms);
//            }

//            // If you had a Faculty Repository, you should also check:
//            // if (await _facultyRepo.AnyInDepartment(department.Id)) return Error...

//            await _departmentRepository.DeleteAsync(department); // Ensure Repository has Delete method
//            await _unitOfWork.SaveChangesAsync(cancellationToken);

//            return Result.Success();
//        }
//    }
//}