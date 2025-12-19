using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeControlSystem.Application.Faculties.TransferDepartment
{
    internal sealed class TransferDepartmentCommandHandler : ICommandHandler<TransferDepartmentCommand>
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TransferDepartmentCommandHandler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork)
        {
            _facultyRepository = facultyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(TransferDepartmentCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetByIdAsync(request.FacultyId, cancellationToken);

            if (faculty is null)
            {
                return Result.Failure(FacultyErrors.NotFound);
            }

            var result = faculty.TransferDepartment(request.NewDepartmentId);

            if (result.IsFailure) return result;

            // TODO: Does i need update function here?
            _facultyRepository.Update(faculty);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
