using CollegeControlSystem.Application.CourseOfferings.ChangeInstructor;
using CollegeControlSystem.Application.CourseOfferings.CreateCourseOffering;
using CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings;
using CollegeControlSystem.Application.CourseOfferings.UpdateOfferingCapacity;
using CollegeControlSystem.Domain.CourseOfferings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.CourseOfferings
{
    [Route("api/course-offerings")]
    [ApiController]
    public sealed class CourseOfferingsController : ControllerBase
    {
        private readonly ISender _sender;

        public CourseOfferingsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourseOffering(
            [FromBody] CreateCourseOfferingRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateCourseOfferingCommand(
                request.CourseId,
                request.InstructorId,
                request.Term,
                request.Year,
                request.Capacity);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            // Returns 201 Created
            // Note: Since we don't have a "GetById" endpoint yet, we just return the ID in the body/location
            return CreatedAtAction(nameof(GetAvailableOfferings), new { term = request.Term, year = request.Year }, result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableOfferings(
            [FromQuery] string term,
            [FromQuery] int year,
            CancellationToken cancellationToken)
        {
            var query = new GetAvailableOfferingsQuery(term, year);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                // Likely invalid semester format (e.g., Year < 2000 or invalid Term string)
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}/capacity")]
        public async Task<IActionResult> UpdateCapacity(
            Guid id,
            [FromBody] UpdateCapacityRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateOfferingCapacityCommand(id, request.NewCapacity);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpPut("{id:guid}/instructor")]
        public async Task<IActionResult> ChangeInstructor(
            Guid id,
            [FromBody] ChangeInstructorRequest request,
            CancellationToken cancellationToken)
        {
            var command = new ChangeInstructorCommand(id, request.NewInstructorId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }

}