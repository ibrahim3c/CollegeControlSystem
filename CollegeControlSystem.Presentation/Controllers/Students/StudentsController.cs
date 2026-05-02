using CollegeControlSystem.Application.Students.AssignAdvisor;
using CollegeControlSystem.Application.Students.CreateStudent;
using CollegeControlSystem.Application.Students.DismissStudent;
using CollegeControlSystem.Application.Students.GetAllStudents;
using CollegeControlSystem.Application.Students.GetStudentProfile;
using CollegeControlSystem.Application.Students.GetStudentsByStatus;
using CollegeControlSystem.Application.Students.GetTranscript;
using CollegeControlSystem.Application.Students.UpdateStudentProfile;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Students
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly ISender _sender;

        public StudentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles=Roles.AdminRole)]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetStudentProfile), new { id = result.Value }, result.Value);
        }

        [HttpGet]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> GetAllStudents([FromQuery] AcademicStatus? status, CancellationToken cancellationToken)
        {
            if (status.HasValue)
            {
                var query = new GetStudentsByStatusQuery(status.Value);
                var result = await _sender.Send(query, cancellationToken);
                return Ok(result.Value);
            }

            var allQuery = new GetAllStudentsQuery();
            var allResult = await _sender.Send(allQuery, cancellationToken);
            return Ok(allResult.Value);
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetStudentProfile(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetStudentProfileQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}/transcript")]
        public async Task<IActionResult> GetTranscript(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetTranscriptQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateStudentProfileRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateStudentProfileCommand(id, request.NewFullName, request.NewNationalId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpPut("{id}/assign-advisor")]
        public async Task<IActionResult> AssignAdvisor(Guid id, [FromBody] AssignAdvisorRequest request, CancellationToken cancellationToken)
        {
            var command = new AssignAdvisorCommand(id, request.AdvisorId);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpPut("{id}/dismiss")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> DismissStudent(Guid id, CancellationToken cancellationToken)
        {
            var command = new DismissStudentCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }


}
