using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs;
using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
    {
        return await _context.Courses.AsNoTracking()
            .Include(c => c.Faculty)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Credits = c.Credits,
                Department = c.Department,
                Semester = c.Semester,
                FacultyId = c.FacultyId,
                FacultyName = c.Faculty != null ? $"{c.Faculty.FirstName} {c.Faculty.LastName}" : null,
                EnrolledStudentsCount = c.Enrollments.Count
            }).ToListAsync();
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        var c = await _context.Courses.AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.Enrollments)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c == null) return null;

        return new CourseDto
        {
            Id = c.Id,
            CourseCode = c.CourseCode,
            CourseName = c.CourseName,
            Credits = c.Credits,
            Department = c.Department,
            Semester = c.Semester,
            FacultyId = c.FacultyId,
            FacultyName = c.Faculty != null ? $"{c.Faculty.FirstName} {c.Faculty.LastName}" : null,
            EnrolledStudentsCount = c.Enrollments.Count
        };
    }

    public async Task<(CourseDto? Course, string? ErrorMessage, bool IsConflict)> CreateCourseAsync(CreateCourseDto dto)
    {
        if (await _context.Courses.AnyAsync(c => c.CourseCode.ToLower() == dto.CourseCode.Trim().ToLower()))
        {
            return (null, $"Course with code '{dto.CourseCode}' already exists.", true);
        }

        if (dto.FacultyId.HasValue && !await _context.FacultyMembers.AnyAsync(f => f.Id == dto.FacultyId.Value))
        {
            return (null, $"Faculty with ID {dto.FacultyId.Value} does not exist.", false);
        }

        var course = new Course
        {
            CourseCode = dto.CourseCode.Trim(),
            CourseName = dto.CourseName.Trim(),
            Credits = dto.Credits,
            Department = dto.Department.Trim(),
            Semester = dto.Semester,
            FacultyId = dto.FacultyId
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        string? facultyName = null;
        if (course.FacultyId.HasValue)
        {
            var f = await _context.FacultyMembers.FindAsync(course.FacultyId.Value);
            if (f != null) facultyName = $"{f.FirstName} {f.LastName}";
        }

        var result = new CourseDto
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Credits = course.Credits,
            Department = course.Department,
            Semester = course.Semester,
            FacultyId = course.FacultyId,
            FacultyName = facultyName,
            EnrolledStudentsCount = 0
        };

        return (result, null, false);
    }

    public async Task<(CourseDto? Course, string? ErrorMessage, bool IsConflict)> UpdateCourseAsync(int id, UpdateCourseDto dto)
    {
        var course = await _context.Courses.Include(c => c.Faculty).FirstOrDefaultAsync(c => c.Id == id);
        if (course == null) return (null, $"Course with ID {id} not found.", false);

        if (dto.FacultyId.HasValue && !await _context.FacultyMembers.AnyAsync(f => f.Id == dto.FacultyId.Value))
        {
            return (null, $"Faculty with ID {dto.FacultyId.Value} does not exist.", false);
        }

        course.CourseName = dto.CourseName.Trim();
        course.Credits = dto.Credits;
        course.Department = dto.Department.Trim();
        course.Semester = dto.Semester;
        course.FacultyId = dto.FacultyId;

        await _context.SaveChangesAsync();

        string? facultyName = null;
        if (course.FacultyId.HasValue)
        {
            var f = await _context.FacultyMembers.FindAsync(course.FacultyId.Value);
            if (f != null) facultyName = $"{f.FirstName} {f.LastName}";
        }

        var enrolledCount = await _context.Enrollments.CountAsync(e => e.CourseId == id);

        var result = new CourseDto
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Credits = course.Credits,
            Department = course.Department,
            Semester = course.Semester,
            FacultyId = course.FacultyId,
            FacultyName = facultyName,
            EnrolledStudentsCount = enrolledCount
        };

        return (result, null, false);
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return true;
    }
}
