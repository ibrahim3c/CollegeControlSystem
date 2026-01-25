using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;
namespace CollegeControlSystem.Application.Courses.AddPrerequisite
{
    internal sealed class AddPrerequisiteCommandHandler : ICommandHandler<AddPrerequisiteCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddPrerequisiteCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddPrerequisiteCommand request, CancellationToken cancellationToken)
        {
            // 1. Load Course
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course is null) return Result.Failure(CourseErrors.CourseNotFound);

            // 2. Check existence of Prerequisite Course (Optional validation step)
            var prereq = await _courseRepository.GetByIdAsync(request.PrerequisiteCourseId, cancellationToken);
            if (prereq is null) return Result.Failure(CourseErrors.PrerequisiteNotFound);

            // 3. Domain Logic
            var result = course.AddPrerequisite(request.PrerequisiteCourseId);

            if (result.IsFailure) return result;

            // 4. Save
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
