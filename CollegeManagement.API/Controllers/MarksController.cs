using CollegeManagement.API.DTOs;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MarksController : ControllerBase
{
    private readonly IMarkService _markService;

    public MarksController(IMarkService markService)
    {
        _markService = markService;
    }

    /// <summary>
    /// Get all student marks/results
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MarkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MarkDto>>> GetMarks()
    {
        var marks = await _markService.GetMarksAsync();
        return Ok(marks);
    }

    /// <summary>
    /// Get mark record by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MarkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarkDto>> GetMark(int id)
    {
        var mark = await _markService.GetMarkByIdAsync(id);
        if (mark == null)
        {
            return NotFound(new { message = $"Mark record with ID {id} was not found." });
        }
        return Ok(mark);
    }

    /// <summary>
    /// Create or update marks for a student in a course with automatic grade calculation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MarkDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MarkDto>> CreateMark([FromBody] CreateMarkDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (mark, errorMessage) = await _markService.CreateMarkAsync(dto);

        if (mark == null)
        {
            return BadRequest(new { message = errorMessage });
        }

        return CreatedAtAction(nameof(GetMark), new { id = mark.Id }, mark);
    }

    /// <summary>
    /// Update existing marks for a student in a course
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MarkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarkDto>> UpdateMark(int id, [FromBody] UpdateMarkDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (mark, errorMessage) = await _markService.UpdateMarkAsync(id, dto);

        if (mark == null)
        {
            return BadRequest(new { message = errorMessage });
        }

        return Ok(mark);
    }

    /// <summary>
    /// Delete a mark record
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMark(int id)
    {
        var success = await _markService.DeleteMarkAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Mark record with ID {id} was not found." });
        }
        return NoContent();
    }
}
