using CollegeControlSystem.Application.Faculties.CreateFaculty;
using CollegeControlSystem.Application.Faculties.GetAdvisorStudents;
using CollegeControlSystem.Application.Faculties.GetFacultyById;
using CollegeControlSystem.Application.Faculties.GetFacultyList;
using CollegeControlSystem.Application.Faculties.GetInstructorCourses;
using CollegeControlSystem.Application.Faculties.TransferDepartment;
using CollegeControlSystem.Application.Faculties.UpdateFacultyInfo;
using MediatR;
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
            /// Retrieves a list of faculty members, optionally filtered by Department.
            /// </summary>
            [HttpGet]
            public async Task<IActionResult> GetFacultyList([FromQuery] Guid? departmentId, CancellationToken cancellationToken)
            {
                var query = new GetFacultyListQuery(departmentId);
                var result = await _sender.Send(query, cancellationToken);

                if (result.IsFailure)
                {
                    return BadRequest(result.Error);
                }

                return Ok(result.Value);
            }

            /// <summary>
            /// Retrieves the profile of a specific faculty member.
            /// </summary>
            [HttpGet("{id}")]
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
            public async Task<IActionResult> UpdateInfo(Guid id, [FromBody] UpdateFacultyInfoRequest request, CancellationToken cancellationToken)
            {
                var command = new UpdateFacultyInfoCommand(id, request.NewDegree);
                var result = await _sender.Send(command, cancellationToken);

                if (result.IsFailure)
                {
                    return NotFound(result.Error); // Could be NotFound or BadRequest depending on error type
                }

                return NoContent();
            }

            /// <summary>
            /// Transfers a faculty member to a different department. (Admin usage)
            /// </summary>
            [HttpPut("{id}/transfer")]
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

            // --- INSTRUCTOR ROLE ENDPOINTS ---

            [HttpGet("{id}/courses")]
            public async Task<IActionResult> GetInstructorCourses(Guid id, CancellationToken cancellationToken)
            {
                var query = new GetInstructorCoursesQuery(id);
                var result = await _sender.Send(query, cancellationToken);

                // Even if the list is empty, it returns Success with an empty list, so we return OK.
                if (result.IsFailure)
                {
                    return BadRequest(result.Error);
                }

                return Ok(result.Value);
            }

            // --- ADVISOR ROLE ENDPOINTS ---

            [HttpGet("{id}/advisees")]
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

}
