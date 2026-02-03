using CollegeControlSystem.Application.Registrations.ApproveRegistration;
using CollegeControlSystem.Application.Registrations.DropCourse;
using CollegeControlSystem.Application.Registrations.GetPendingRegistrations;
using CollegeControlSystem.Application.Registrations.GetStudentSchedule;
using CollegeControlSystem.Application.Registrations.RegisterCourse;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;  
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Registratoins
{
    [Route("api/registrations")]
    [ApiController]
    public sealed class RegistrationsController : ControllerBase
    {
        private readonly ISender _sender;

        public RegistrationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCourse(
            [FromBody] RegisterCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new RegisterCourseCommand(
                request.StudentId,
                request.CourseOfferingId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // Map Domain Errors to specific HTTP Status Codes for better client handling
                if (result.Error == RegistrationErrors.DuplicateRegistration)
                {
                    return Conflict(result.Error); // 409 Conflict
                }

                if (result.Error == StudentErrors.StudentNotFound ||
                    result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error); // 404 Not Found
                }

                // Default for Prerequisite failure, Overload, Capacity full, etc.
                return BadRequest(result.Error); // 400 Bad Request
            }

            // Return 201 Created
            // We use the GetStudentSchedule endpoint as the "Location" for the new resource context
            return CreatedAtAction(
                nameof(GetStudentSchedule),
                new { studentId = request.StudentId },
                new { registrationId = result.Value });
        }

        [HttpPut("{id:guid}/approve")]
        public async Task<IActionResult> ApproveRegistration(
            Guid id,
            [FromBody] ApproveRegistrationRequest request,
            CancellationToken cancellationToken)
        {
            // 'id' is the RegistrationId from the URL
            var command = new ApproveRegistrationCommand(id, request.AdvisorId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == RegistrationErrors.Unauthorized)
                {
                    return StatusCode(403, result.Error); // 403 Forbidden
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpPut("{id:guid}/drop")]
        public async Task<IActionResult> DropCourse(
            Guid id,
            [FromBody] DropCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new DropCourseCommand(id, request.StudentId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == RegistrationErrors.Unauthorized)
                {
                    return StatusCode(403, result.Error);
                }

                // e.g., Cannot drop a course that is already graded/completed
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRegistrations(
            [FromQuery] Guid advisorId,
            CancellationToken cancellationToken)
        {
            var query = new GetPendingRegistrationsQuery(advisorId);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("schedule/{studentId:guid}")]
        public async Task<IActionResult> GetStudentSchedule(
            Guid studentId,
            [FromQuery] string? term,
            [FromQuery] int? year,
            CancellationToken cancellationToken)
        {
            var query = new GetStudentScheduleQuery(studentId, term, year);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == StudentErrors.StudentNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }

}