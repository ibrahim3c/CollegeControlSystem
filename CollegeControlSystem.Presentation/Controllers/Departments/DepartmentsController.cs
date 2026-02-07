using CollegeControlSystem.Application.Departments.AddProgram;
using CollegeControlSystem.Application.Departments.CreateDepartment;
using CollegeControlSystem.Application.Departments.GetDepartments;
using CollegeControlSystem.Application.Departments.GetPrograms;
using CollegeControlSystem.Application.Departments.UpdateDepartment;
using CollegeControlSystem.Application.Departments.UpdateProgramCredits;
using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Infrastructure.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Departments
{
    [Route("api/departments")]
    [ApiController]
    public sealed class DepartmentsController : ControllerBase
    {
        private readonly ISender _sender;

        public DepartmentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> CreateDepartment(
            [FromBody] CreateDepartmentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateDepartmentCommand(request.Name, request.Description);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            // Returns 201 Created with the location header (optional) and the ID
            return CreatedAtAction(nameof(GetDepartments), new { id = result.Value }, result.Value);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
        {
            var query = new GetDepartmentsQuery();

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                // Queries usually don't fail in this way, but good safety net
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateDepartment(
            Guid id,
            [FromBody] UpdateDepartmentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateDepartmentCommand(id, request.Name, request.Description);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // Check if the error is specifically a "Not Found" error to return 404
                if (result.Error == DepartmentErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent(); // Standard 204 response for updates
        }

        // --- Program Sub-Resources ---

        [HttpPost("{departmentId:guid}/programs")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> AddProgram(
            Guid departmentId,
            [FromBody] AddProgramRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AddProgramCommand(
                departmentId,
                request.Name,
                request.RequiredCredits);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == DepartmentErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("programs")]
        [Authorize]
        public async Task<IActionResult> GetAllPrograms(CancellationToken cancellationToken)
        {
            var query = new GetProgramsQuery();

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPut("{departmentId:guid}/programs/{programId:guid}/credits")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateProgramCredits(
            Guid departmentId,
            Guid programId,
            [FromBody] UpdateProgramCreditsRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateProgramCreditsCommand(
                departmentId,
                programId,
                request.NewRequiredCredits);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == DepartmentErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}