using CollegeControlSystem.Application.Students.AssignAdvisor;
using CollegeControlSystem.Application.Students.CreateStudent;
using CollegeControlSystem.Application.Students.GetStudentProfile;
using CollegeControlSystem.Application.Students.GetTranscript;
using CollegeControlSystem.Application.Students.UpdateStudentProfile;
using MediatR;
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
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            // Returns 201 Created with a Location header pointing to the GetProfile endpoint
            return CreatedAtAction(nameof(GetStudentProfile), new { id = result.Value }, result.Value);
        }

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
            // Bind Route ID and Body properties to the Command
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
    }


}
