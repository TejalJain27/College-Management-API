using CollegeManagement.API.DTOs;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Get all courses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _courseService.GetCoursesAsync();
        return Ok(courses);
    }

    /// <summary>
    /// Get a course by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound(new { message = $"Course with ID {id} was not found." });
        }
        return Ok(course);
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (course, errorMessage, isConflict) = await _courseService.CreateCourseAsync(dto);

        if (isConflict)
        {
            return Conflict(new { message = errorMessage });
        }

        if (course == null)
        {
            return BadRequest(new { message = errorMessage });
        }

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
    }

    /// <summary>
    /// Update an existing course
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CourseDto>> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (course, errorMessage, isConflict) = await _courseService.UpdateCourseAsync(id, dto);

        if (isConflict)
        {
            return Conflict(new { message = errorMessage });
        }

        if (course == null)
        {
            return NotFound(new { message = errorMessage });
        }

        return Ok(course);
    }

    /// <summary>
    /// Delete a course
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var success = await _courseService.DeleteCourseAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Course with ID {id} was not found." });
        }
        return NoContent();
    }
}
