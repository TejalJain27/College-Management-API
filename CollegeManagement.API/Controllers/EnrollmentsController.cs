using CollegeManagement.API.DTOs;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Get all enrollments
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollments()
    {
        var enrollments = await _enrollmentService.GetEnrollmentsAsync();
        return Ok(enrollments);
    }

    /// <summary>
    /// Get enrollment by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollment(int id)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound(new { message = $"Enrollment with ID {id} was not found." });
        }
        return Ok(enrollment);
    }

    /// <summary>
    /// Create a new enrollment (enroll a student in a course)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollment([FromBody] CreateEnrollmentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (enrollment, errorMessage, isConflict) = await _enrollmentService.CreateEnrollmentAsync(dto);

        if (isConflict)
        {
            return Conflict(new { message = errorMessage });
        }

        if (enrollment == null)
        {
            return BadRequest(new { message = errorMessage });
        }

        return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollment);
    }

    /// <summary>
    /// Update enrollment status
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentDto>> UpdateEnrollment(int id, [FromBody] UpdateEnrollmentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (enrollment, errorMessage, _) = await _enrollmentService.UpdateEnrollmentAsync(id, dto);

        if (enrollment == null)
        {
            return NotFound(new { message = errorMessage });
        }

        return Ok(enrollment);
    }

    /// <summary>
    /// Delete an enrollment
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        var success = await _enrollmentService.DeleteEnrollmentAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Enrollment with ID {id} was not found." });
        }
        return NoContent();
    }
}
