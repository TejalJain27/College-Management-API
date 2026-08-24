using CollegeManagement.API.DTOs;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Get list of students with optional search, department, and semester filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents(
        [FromQuery] string? search,
        [FromQuery] string? department,
        [FromQuery] int? semester)
    {
        var students = await _studentService.GetStudentsAsync(search, department, semester);
        return Ok(students);
    }

    /// <summary>
    /// Get a student by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetStudent(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} was not found." });
        }
        return Ok(student);
    }

    /// <summary>
    /// Create a new student
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] CreateStudentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (student, errorMessage, isConflict) = await _studentService.CreateStudentAsync(dto);

        if (isConflict)
        {
            return Conflict(new { message = errorMessage });
        }

        if (student == null)
        {
            return BadRequest(new { message = errorMessage });
        }

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }

    /// <summary>
    /// Update an existing student
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StudentDto>> UpdateStudent(int id, [FromBody] UpdateStudentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (student, errorMessage, isConflict) = await _studentService.UpdateStudentAsync(id, dto);

        if (isConflict)
        {
            return Conflict(new { message = errorMessage });
        }

        if (student == null)
        {
            return NotFound(new { message = errorMessage });
        }

        return Ok(student);
    }

    /// <summary>
    /// Delete a student
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var success = await _studentService.DeleteStudentAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Student with ID {id} was not found." });
        }
        return NoContent();
    }
}
