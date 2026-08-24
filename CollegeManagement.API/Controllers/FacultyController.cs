using CollegeManagement.API.DTOs;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FacultyController : ControllerBase
{
    private readonly IFacultyService _facultyService;

    public FacultyController(IFacultyService facultyService)
    {
        _facultyService = facultyService;
    }

    /// <summary>
    /// Get all faculty members
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FacultyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FacultyDto>>> GetFaculty()
    {
        var faculty = await _facultyService.GetFacultyAsync();
        return Ok(faculty);
    }

    /// <summary>
    /// Get a faculty member by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FacultyDto>> GetFaculty(int id)
    {
        var faculty = await _facultyService.GetFacultyByIdAsync(id);
        if (faculty == null)
        {
            return NotFound(new { message = $"Faculty member with ID {id} was not found." });
        }
        return Ok(faculty);
    }

    /// <summary>
    /// Create a new faculty member
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FacultyDto>> CreateFaculty([FromBody] CreateFacultyDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (faculty, errorMessage, isConflict) = await _facultyService.CreateFacultyAsync(dto);

        if (isConflict)
        {
            return Conflict(new { message = errorMessage });
        }

        if (faculty == null)
        {
            return BadRequest(new { message = errorMessage });
        }

        return CreatedAtAction(nameof(GetFaculty), new { id = faculty.Id }, faculty);
    }

    /// <summary>
    /// Update an existing faculty member
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FacultyDto>> UpdateFaculty(int id, [FromBody] UpdateFacultyDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (faculty, errorMessage, isConflict) = await _facultyService.UpdateFacultyAsync(id, dto);

        if (isConflict)
        {
            return Conflict(new { message = errorMessage });
        }

        if (faculty == null)
        {
            return NotFound(new { message = errorMessage });
        }

        return Ok(faculty);
    }

    /// <summary>
    /// Delete a faculty member
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFaculty(int id)
    {
        var success = await _facultyService.DeleteFacultyAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Faculty member with ID {id} was not found." });
        }
        return NoContent();
    }
}
