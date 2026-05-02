using CollegeControlSystem.Application.Faculties.ChangeFacultyStatus;
using CollegeControlSystem.Application.Faculties.CreateFaculty;
using CollegeControlSystem.Application.Faculties.GetAdvisorStudents;
using CollegeControlSystem.Application.Faculties.GetFacultyById;
using CollegeControlSystem.Application.Faculties.GetFacultyList;
using CollegeControlSystem.Application.Faculties.GetFacultiesByStatus;
using CollegeControlSystem.Application.Faculties.GetInstructorCourses;
using CollegeControlSystem.Application.Faculties.TransferDepartment;
using CollegeControlSystem.Application.Faculties.UpdateFacultyInfo;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Faculties
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacultyController : ControllerBase
    {
        private readonly ISender _sender;

        public FacultyController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new faculty member. (Admin usage)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetFacultyById), new { id = result.Value }, result.Value);
        }

        /// <summary>
        /// Retrieves a list of faculty members, optionally filtered by Department or Status.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFacultyList(
            [FromQuery] Guid? departmentId,
            [FromQuery] FacultyStatus? status,
            CancellationToken cancellationToken)
        {
            if (status.HasValue)
            {
                var query = new GetFacultiesByStatusQuery(status.Value);
                var result = await _sender.Send(query, cancellationToken);
                return Ok(result.Value);
            }

            var query2 = new GetFacultyListQuery(departmentId);
            var result2 = await _sender.Send(query2, cancellationToken);

            if (result2.IsFailure)
            {
                return BadRequest(result2.Error);
            }

            return Ok(result2.Value);
        }

        /// <summary>
        /// Retrieves the profile of a specific faculty member.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetFacultyById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetFacultyByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates a faculty member's degree (e.g., "PhD", "Professor").
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateInfo(Guid id, [FromBody] UpdateFacultyInfoRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateFacultyInfoCommand(id, request.NewDegree);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Transfers a faculty member to a different department. (Admin usage)
        /// </summary>
        [HttpPut("{id}/transfer")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> TransferDepartment(Guid id, [FromBody] TransferDepartmentRequest request, CancellationToken cancellationToken)
        {
            var command = new TransferDepartmentCommand(id, request.NewDepartmentId);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Changes a faculty member's status (Active, Resigned, Retired, Dismissed).
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeFacultyStatusRequest request, CancellationToken cancellationToken)
        {
            var command = new ChangeFacultyStatusCommand(id, request.NewStatus);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        // --- INSTRUCTOR ROLE ENDPOINTS ---

        [HttpGet("{id}/courses")]
        [Authorize(Roles = Roles.ProfessorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetInstructorCourses(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetInstructorCoursesQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        // --- ADVISOR ROLE ENDPOINTS ---

        [HttpGet("{id}/advisees")]
        [Authorize(Roles = Roles.AdvisorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetAdvisorStudents(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetAdvisorStudentsQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }

    public record ChangeFacultyStatusRequest(FacultyStatus NewStatus);
}
