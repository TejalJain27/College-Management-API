using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs;
using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services;

public class MarkService : IMarkService
{
    private readonly ApplicationDbContext _context;

    public MarkService(ApplicationDbContext context)
    {
        _context = context;
    }

    public static string CalculateGrade(decimal marksObtained, decimal maximumMarks)
    {
        if (maximumMarks <= 0) return "F";
        var percentage = (marksObtained / maximumMarks) * 100;

        return percentage switch
        {
            >= 90 => "A+",
            >= 80 => "A",
            >= 70 => "B",
            >= 60 => "C",
            >= 50 => "D",
            _ => "F"
        };
    }

    public async Task<IEnumerable<MarkDto>> GetMarksAsync()
    {
        return await _context.Marks.AsNoTracking()
            .Include(m => m.Student)
            .Include(m => m.Course)
            .Select(m => new MarkDto
            {
                Id = m.Id,
                StudentId = m.StudentId,
                StudentNumber = m.Student.StudentNumber,
                StudentName = $"{m.Student.FirstName} {m.Student.LastName}",
                CourseId = m.CourseId,
                CourseCode = m.Course.CourseCode,
                CourseName = m.Course.CourseName,
                MarksObtained = m.MarksObtained,
                MaximumMarks = m.MaximumMarks,
                Grade = m.Grade
            }).ToListAsync();
    }

    public async Task<MarkDto?> GetMarkByIdAsync(int id)
    {
        var m = await _context.Marks.AsNoTracking()
            .Include(x => x.Student)
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (m == null) return null;

        return new MarkDto
        {
            Id = m.Id,
            StudentId = m.StudentId,
            StudentNumber = m.Student.StudentNumber,
            StudentName = $"{m.Student.FirstName} {m.Student.LastName}",
            CourseId = m.CourseId,
            CourseCode = m.Course.CourseCode,
            CourseName = m.Course.CourseName,
            MarksObtained = m.MarksObtained,
            MaximumMarks = m.MaximumMarks,
            Grade = m.Grade
        };
    }

    public async Task<(MarkDto? Mark, string? ErrorMessage)> CreateMarkAsync(CreateMarkDto dto)
    {
        if (dto.MarksObtained < 0)
        {
            return (null, "Marks obtained cannot be negative.");
        }

        if (dto.MaximumMarks <= 0)
        {
            return (null, "Maximum marks must be greater than zero.");
        }

        if (dto.MarksObtained > dto.MaximumMarks)
        {
            return (null, $"Marks obtained ({dto.MarksObtained}) cannot be greater than maximum marks ({dto.MaximumMarks}).");
        }

        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null) return (null, $"Student with ID {dto.StudentId} does not exist.");

        var course = await _context.Courses.FindAsync(dto.CourseId);
        if (course == null) return (null, $"Course with ID {dto.CourseId} does not exist.");

        // Check if mark already recorded for this student & course
        var existing = await _context.Marks
            .FirstOrDefaultAsync(m => m.StudentId == dto.StudentId && m.CourseId == dto.CourseId);

        var grade = CalculateGrade(dto.MarksObtained, dto.MaximumMarks);

        if (existing != null)
        {
            existing.MarksObtained = dto.MarksObtained;
            existing.MaximumMarks = dto.MaximumMarks;
            existing.Grade = grade;
            await _context.SaveChangesAsync();

            return (new MarkDto
            {
                Id = existing.Id,
                StudentId = student.Id,
                StudentNumber = student.StudentNumber,
                StudentName = $"{student.FirstName} {student.LastName}",
                CourseId = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                MarksObtained = existing.MarksObtained,
                MaximumMarks = existing.MaximumMarks,
                Grade = existing.Grade
            }, null);
        }

        var mark = new Mark
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            MarksObtained = dto.MarksObtained,
            MaximumMarks = dto.MaximumMarks,
            Grade = grade
        };

        _context.Marks.Add(mark);
        await _context.SaveChangesAsync();

        var result = new MarkDto
        {
            Id = mark.Id,
            StudentId = student.Id,
            StudentNumber = student.StudentNumber,
            StudentName = $"{student.FirstName} {student.LastName}",
            CourseId = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            MarksObtained = mark.MarksObtained,
            MaximumMarks = mark.MaximumMarks,
            Grade = mark.Grade
        };

        return (result, null);
    }

    public async Task<(MarkDto? Mark, string? ErrorMessage)> UpdateMarkAsync(int id, UpdateMarkDto dto)
    {
        var mark = await _context.Marks
            .Include(m => m.Student)
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mark == null) return (null, $"Mark record with ID {id} not found.");

        if (dto.MarksObtained < 0) return (null, "Marks obtained cannot be negative.");
        if (dto.MaximumMarks <= 0) return (null, "Maximum marks must be greater than zero.");
        if (dto.MarksObtained > dto.MaximumMarks) return (null, $"Marks obtained ({dto.MarksObtained}) cannot exceed maximum marks ({dto.MaximumMarks}).");

        mark.MarksObtained = dto.MarksObtained;
        mark.MaximumMarks = dto.MaximumMarks;
        mark.Grade = CalculateGrade(dto.MarksObtained, dto.MaximumMarks);

        await _context.SaveChangesAsync();

        var result = new MarkDto
        {
            Id = mark.Id,
            StudentId = mark.StudentId,
            StudentNumber = mark.Student.StudentNumber,
            StudentName = $"{mark.Student.FirstName} {mark.Student.LastName}",
            CourseId = mark.CourseId,
            CourseCode = mark.Course.CourseCode,
            CourseName = mark.Course.CourseName,
            MarksObtained = mark.MarksObtained,
            MaximumMarks = mark.MaximumMarks,
            Grade = mark.Grade
        };

        return (result, null);
    }

    public async Task<bool> DeleteMarkAsync(int id)
    {
        var mark = await _context.Marks.FindAsync(id);
        if (mark == null) return false;

        _context.Marks.Remove(mark);
        await _context.SaveChangesAsync();
        return true;
    }
}
