using CollegeControlSystem.Application.Registrations.ApproveRegistration;
using CollegeControlSystem.Application.Registrations.DropCourse;
using CollegeControlSystem.Application.Registrations.GetAvailableCourses;
using CollegeControlSystem.Application.Registrations.GetPendingRegistrations;
using CollegeControlSystem.Application.Registrations.GetRegistrationById;
using CollegeControlSystem.Application.Registrations.GetStudentRegistrations;
using CollegeControlSystem.Application.Registrations.GetStudentSchedule;
using CollegeControlSystem.Application.Registrations.RegisterCourse;
using CollegeControlSystem.Application.Registrations.SubmitGrades;
using CollegeControlSystem.Application.Registrations.WithdrawCourse;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;  
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = Roles.StudentRole)]
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
        [Authorize(Roles = Roles.AdvisorRole + "," + Roles.AdminRole)]
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
        [Authorize(Roles = Roles.StudentRole)]
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

        [HttpPut("{id:guid}/withdraw")]
        [Authorize(Roles = Roles.StudentRole)]
        public async Task<IActionResult> WithdrawCourse(
            Guid id,
            [FromBody] WithdrawCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new WithdrawCourseCommand(id, request.StudentId);

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

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpGet("pending")]
        [Authorize(Roles = Roles.AdvisorRole + "," + Roles.AdminRole)]
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
        [Authorize(Roles = Roles.StudentRole)]
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

        /// <summary>
        /// Shows students the available courses they can register for in the specified semester.
        /// </summary>
        [HttpGet("available")]
        [Authorize(Roles = Roles.StudentRole)] // Only Students access their registration board
        public async Task<IActionResult> GetAvailableCoursesForRegistration(
            [FromQuery] Guid studentId,
            [FromQuery] string term,
            [FromQuery] int year,
            CancellationToken cancellationToken)
        {
            var query = new GetAvailableCoursesQuery(studentId, term, year);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == StudentErrors.StudentNotFound)
                {
                    return NotFound(result.Error);
                }

                // E.g., Invalid Semester Term provided
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("student/{studentId:guid}")]
        [Authorize(Roles = Roles.StudentRole + "," + Roles.AdvisorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetStudentRegistrations(
            Guid studentId,
            CancellationToken cancellationToken)
        {
            var query = new GetStudentRegistrationsQuery(studentId);

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

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetRegistrationByIdQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Submits semester work and final exam grades for students.
        /// Automatically calculates total points, letter grades, and applies retake caps.
        /// </summary>
        [HttpPost("submit")]
        [Authorize(Roles = Roles.ProfessorRole + "," + Roles.AdminRole)] // Instructors submit grades
        public async Task<IActionResult> SubmitGrades(
            [FromBody] SubmitGradesRequest request,
            CancellationToken cancellationToken)
        {
            // Security Best Practice: Extract the Instructor ID from the JWT Claims
            // ClaimTypes.NameIdentifier usually stores the User ID when generating the token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid instructorId))
            {
                return Unauthorized(new { Error = "Invalid user token." });
            }

            var command = new SubmitGradesCommand(
                request.OfferingId,
                instructorId,
                request.Submissions);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // E.g., Returns 400 if scores exceed 100 or are negative
                return BadRequest(result.Error);
            }

            return Ok(new { Message = "Grades successfully submitted and applied." });
        }
    }
}