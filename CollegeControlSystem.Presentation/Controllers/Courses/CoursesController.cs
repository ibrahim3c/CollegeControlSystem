using CollegeControlSystem.Application.Courses.AddPrerequisite;
using CollegeControlSystem.Application.Courses.CreateCourse;
using CollegeControlSystem.Application.Courses.GetCourseDetails;
using CollegeControlSystem.Application.Courses.GetCourseList;
using CollegeControlSystem.Application.Courses.RemovePrerequisite;
using CollegeControlSystem.Domain.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Courses
{
    [Route("api/courses")]
    [ApiController]
    public sealed class CoursesController : ControllerBase
    {
        private readonly ISender _sender;

        public CoursesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(
            [FromBody] CreateCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateCourseCommand(
                request.DepartmentId,
                request.Code,
                request.Title,
                request.Description,
                request.Credits,
                request.LectureHours,
                request.LabHours);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // Handle duplicate code specifically if needed
                if (result.Error == CourseErrors.DuplicateCode)
                {
                    return Conflict(result.Error); // HTTP 409 Conflict
                }

                return BadRequest(result.Error);
            }

            // Return 201 Created with a Location header pointing to the GetById endpoint
            return CreatedAtAction(nameof(GetCourseDetails), new { id = result.Value }, result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseList(
            [FromQuery] Guid? departmentId,
            CancellationToken cancellationToken)
        {
            var query = new GetCourseListQuery(departmentId);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCourseDetails(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetCourseDetailsQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        // --- Prerequisite Sub-Resources ---

        [HttpPost("{id:guid}/prerequisites")]
        public async Task<IActionResult> AddPrerequisite(
            Guid id,
            [FromBody] AddPrerequisiteRequest request,
            CancellationToken cancellationToken)
        {
            // 'id' from route is the Course, 'request.PrerequisiteCourseId' is the dependency
            var command = new AddPrerequisiteCommand(id, request.PrerequisiteCourseId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound ||
                    result.Error == CourseErrors.PrerequisiteNotFound)
                {
                    return NotFound(result.Error);
                }

                // e.g. CircularDependency error
                return BadRequest(result.Error);
            }

            return NoContent(); // 204 No Content is standard for "Action completed, nothing to return"
        }

        [HttpDelete("{id:guid}/prerequisites/{prerequisiteId:guid}")]
        public async Task<IActionResult> RemovePrerequisite(
            Guid id,
            Guid prerequisiteId,
            CancellationToken cancellationToken)
        {
            var command = new RemovePrerequisiteCommand(id, prerequisiteId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}