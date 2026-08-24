using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs;
using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;

    public EnrollmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsAsync()
    {
        return await _context.Enrollments.AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Select(e => new EnrollmentDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentNumber = e.Student.StudentNumber,
                StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                CourseId = e.CourseId,
                CourseCode = e.Course.CourseCode,
                CourseName = e.Course.CourseName,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status
            }).ToListAsync();
    }

    public async Task<EnrollmentDto?> GetEnrollmentByIdAsync(int id)
    {
        var e = await _context.Enrollments.AsNoTracking()
            .Include(x => x.Student)
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e == null) return null;

        return new EnrollmentDto
        {
            Id = e.Id,
            StudentId = e.StudentId,
            StudentNumber = e.Student.StudentNumber,
            StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
            CourseId = e.CourseId,
            CourseCode = e.Course.CourseCode,
            CourseName = e.Course.CourseName,
            EnrollmentDate = e.EnrollmentDate,
            Status = e.Status
        };
    }

    public async Task<(EnrollmentDto? Enrollment, string? ErrorMessage, bool IsConflict)> CreateEnrollmentAsync(CreateEnrollmentDto dto)
    {
        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null)
        {
            return (null, $"Student with ID {dto.StudentId} does not exist.", false);
        }

        var course = await _context.Courses.FindAsync(dto.CourseId);
        if (course == null)
        {
            return (null, $"Course with ID {dto.CourseId} does not exist.", false);
        }

        // Duplicate check
        if (await _context.Enrollments.AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId))
        {
            return (null, $"Student '{student.FirstName} {student.LastName}' is already enrolled in course '{course.CourseCode}'.", true);
        }

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            EnrollmentDate = DateTime.UtcNow,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status.Trim()
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        var result = new EnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = student.Id,
            StudentNumber = student.StudentNumber,
            StudentName = $"{student.FirstName} {student.LastName}",
            CourseId = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status
        };

        return (result, null, false);
    }

    public async Task<(EnrollmentDto? Enrollment, string? ErrorMessage, bool IsConflict)> UpdateEnrollmentAsync(int id, UpdateEnrollmentDto dto)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null) return (null, $"Enrollment with ID {id} not found.", false);

        enrollment.Status = dto.Status.Trim();
        await _context.SaveChangesAsync();

        var result = new EnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            StudentNumber = enrollment.Student.StudentNumber,
            StudentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}",
            CourseId = enrollment.CourseId,
            CourseCode = enrollment.Course.CourseCode,
            CourseName = enrollment.Course.CourseName,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status
        };

        return (result, null, false);
    }

    public async Task<bool> DeleteEnrollmentAsync(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return false;

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return true;
    }
}
