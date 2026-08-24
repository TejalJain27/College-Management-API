using CollegeManagement.API.DTOs;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    /// <summary>
    /// Get attendance records with optional student and course filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AttendanceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAttendance(
        [FromQuery] int? studentId,
        [FromQuery] int? courseId)
    {
        var records = await _attendanceService.GetAttendanceRecordsAsync(studentId, courseId);
        return Ok(records);
    }

    /// <summary>
    /// Get attendance record by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AttendanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttendanceDto>> GetAttendance(int id)
    {
        var record = await _attendanceService.GetAttendanceByIdAsync(id);
        if (record == null)
        {
            return NotFound(new { message = $"Attendance record with ID {id} was not found." });
        }
        return Ok(record);
    }

    /// <summary>
    /// Get attendance summary and percentage for a student in a course
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(AttendanceSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttendanceSummaryDto>> GetAttendanceSummary(
        [FromQuery] int studentId,
        [FromQuery] int courseId)
    {
        var summary = await _attendanceService.GetStudentAttendanceSummaryAsync(studentId, courseId);
        if (summary == null)
        {
            return NotFound(new { message = "Student or Course record not found." });
        }
        return Ok(summary);
    }

    /// <summary>
    /// Create or update attendance record for a student on a specific date
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AttendanceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AttendanceDto>> CreateAttendance([FromBody] CreateAttendanceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (attendance, errorMessage) = await _attendanceService.CreateAttendanceAsync(dto);

        if (attendance == null)
        {
            return BadRequest(new { message = errorMessage });
        }

        return CreatedAtAction(nameof(GetAttendance), new { id = attendance.Id }, attendance);
    }

    /// <summary>
    /// Update an existing attendance record
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AttendanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttendanceDto>> UpdateAttendance(int id, [FromBody] UpdateAttendanceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (attendance, errorMessage) = await _attendanceService.UpdateAttendanceAsync(id, dto);

        if (attendance == null)
        {
            return NotFound(new { message = errorMessage });
        }

        return Ok(attendance);
    }

    /// <summary>
    /// Delete an attendance record
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttendance(int id)
    {
        var success = await _attendanceService.DeleteAttendanceAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Attendance record with ID {id} was not found." });
        }
        return NoContent();
    }
}
