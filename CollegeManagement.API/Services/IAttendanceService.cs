using CollegeManagement.API.DTOs;

namespace CollegeManagement.API.Services;

public interface IAttendanceService
{
    Task<IEnumerable<AttendanceDto>> GetAttendanceRecordsAsync(int? studentId = null, int? courseId = null);
    Task<AttendanceDto?> GetAttendanceByIdAsync(int id);
    Task<AttendanceSummaryDto?> GetStudentAttendanceSummaryAsync(int studentId, int courseId);
    Task<(AttendanceDto? Attendance, string? ErrorMessage)> CreateAttendanceAsync(CreateAttendanceDto dto);
    Task<(AttendanceDto? Attendance, string? ErrorMessage)> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto);
    Task<bool> DeleteAttendanceAsync(int id);
}
