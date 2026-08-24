using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs;
using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;

    public AttendanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AttendanceDto>> GetAttendanceRecordsAsync(int? studentId = null, int? courseId = null)
    {
        var query = _context.AttendanceRecords.AsNoTracking()
            .Include(a => a.Student)
            .Include(a => a.Course)
            .AsQueryable();

        if (studentId.HasValue)
        {
            query = query.Where(a => a.StudentId == studentId.Value);
        }

        if (courseId.HasValue)
        {
            query = query.Where(a => a.CourseId == courseId.Value);
        }

        return await query.OrderByDescending(a => a.Date).Select(a => new AttendanceDto
        {
            Id = a.Id,
            StudentId = a.StudentId,
            StudentNumber = a.Student.StudentNumber,
            StudentName = $"{a.Student.FirstName} {a.Student.LastName}",
            CourseId = a.CourseId,
            CourseCode = a.Course.CourseCode,
            CourseName = a.Course.CourseName,
            Date = a.Date,
            IsPresent = a.IsPresent
        }).ToListAsync();
    }

    public async Task<AttendanceDto?> GetAttendanceByIdAsync(int id)
    {
        var a = await _context.AttendanceRecords.AsNoTracking()
            .Include(x => x.Student)
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (a == null) return null;

        return new AttendanceDto
        {
            Id = a.Id,
            StudentId = a.StudentId,
            StudentNumber = a.Student.StudentNumber,
            StudentName = $"{a.Student.FirstName} {a.Student.LastName}",
            CourseId = a.CourseId,
            CourseCode = a.Course.CourseCode,
            CourseName = a.Course.CourseName,
            Date = a.Date,
            IsPresent = a.IsPresent
        };
    }

    public async Task<AttendanceSummaryDto?> GetStudentAttendanceSummaryAsync(int studentId, int courseId)
    {
        var student = await _context.Students.FindAsync(studentId);
        var course = await _context.Courses.FindAsync(courseId);

        if (student == null || course == null) return null;

        var records = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId && a.CourseId == courseId)
            .ToListAsync();

        int total = records.Count;
        int present = records.Count(r => r.IsPresent);

        return new AttendanceSummaryDto
        {
            StudentId = studentId,
            StudentName = $"{student.FirstName} {student.LastName}",
            CourseId = courseId,
            CourseCode = course.CourseCode,
            TotalClasses = total,
            PresentClasses = present
        };
    }

    public async Task<(AttendanceDto? Attendance, string? ErrorMessage)> CreateAttendanceAsync(CreateAttendanceDto dto)
    {
        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null) return (null, $"Student with ID {dto.StudentId} does not exist.");

        var course = await _context.Courses.FindAsync(dto.CourseId);
        if (course == null) return (null, $"Course with ID {dto.CourseId} does not exist.");

        var dateUtc = DateTime.SpecifyKind(dto.Date.Date, DateTimeKind.Utc);

        // Check if attendance already logged for this student, course, and date
        var existing = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.StudentId == dto.StudentId && a.CourseId == dto.CourseId && a.Date == dateUtc);

        if (existing != null)
        {
            existing.IsPresent = dto.IsPresent;
            await _context.SaveChangesAsync();

            return (new AttendanceDto
            {
                Id = existing.Id,
                StudentId = student.Id,
                StudentNumber = student.StudentNumber,
                StudentName = $"{student.FirstName} {student.LastName}",
                CourseId = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Date = existing.Date,
                IsPresent = existing.IsPresent
            }, null);
        }

        var attendance = new Attendance
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            Date = dateUtc,
            IsPresent = dto.IsPresent
        };

        _context.AttendanceRecords.Add(attendance);
        await _context.SaveChangesAsync();

        var result = new AttendanceDto
        {
            Id = attendance.Id,
            StudentId = student.Id,
            StudentNumber = student.StudentNumber,
            StudentName = $"{student.FirstName} {student.LastName}",
            CourseId = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Date = attendance.Date,
            IsPresent = attendance.IsPresent
        };

        return (result, null);
    }

    public async Task<(AttendanceDto? Attendance, string? ErrorMessage)> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto)
    {
        var attendance = await _context.AttendanceRecords
            .Include(a => a.Student)
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attendance == null) return (null, $"Attendance record with ID {id} not found.");

        attendance.Date = DateTime.SpecifyKind(dto.Date.Date, DateTimeKind.Utc);
        attendance.IsPresent = dto.IsPresent;

        await _context.SaveChangesAsync();

        var result = new AttendanceDto
        {
            Id = attendance.Id,
            StudentId = attendance.StudentId,
            StudentNumber = attendance.Student.StudentNumber,
            StudentName = $"{attendance.Student.FirstName} {attendance.Student.LastName}",
            CourseId = attendance.CourseId,
            CourseCode = attendance.Course.CourseCode,
            CourseName = attendance.Course.CourseName,
            Date = attendance.Date,
            IsPresent = attendance.IsPresent
        };

        return (result, null);
    }

    public async Task<bool> DeleteAttendanceAsync(int id)
    {
        var attendance = await _context.AttendanceRecords.FindAsync(id);
        if (attendance == null) return false;

        _context.AttendanceRecords.Remove(attendance);
        await _context.SaveChangesAsync();
        return true;
    }
}
