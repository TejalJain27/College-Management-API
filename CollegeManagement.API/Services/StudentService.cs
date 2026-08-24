using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs;
using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsAsync(string? search = null, string? department = null, int? semester = null)
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(s =>
                s.StudentNumber.ToLower().Contains(term) ||
                s.FirstName.ToLower().Contains(term) ||
                s.LastName.ToLower().Contains(term) ||
                s.Email.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(s => s.Department.ToLower() == department.Trim().ToLower());
        }

        if (semester.HasValue)
        {
            query = query.Where(s => s.Semester == semester.Value);
        }

        return await query.Select(s => new StudentDto
        {
            Id = s.Id,
            StudentNumber = s.StudentNumber,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            Phone = s.Phone,
            DateOfBirth = s.DateOfBirth,
            Department = s.Department,
            Semester = s.Semester,
            CreatedAt = s.CreatedAt
        }).ToListAsync();
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var s = await _context.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return null;

        return new StudentDto
        {
            Id = s.Id,
            StudentNumber = s.StudentNumber,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            Phone = s.Phone,
            DateOfBirth = s.DateOfBirth,
            Department = s.Department,
            Semester = s.Semester,
            CreatedAt = s.CreatedAt
        };
    }

    public async Task<(StudentDto? Student, string? ErrorMessage, bool IsConflict)> CreateStudentAsync(CreateStudentDto dto)
    {
        // Unique validation
        if (await _context.Students.AnyAsync(s => s.StudentNumber.ToLower() == dto.StudentNumber.Trim().ToLower()))
        {
            return (null, $"Student with number '{dto.StudentNumber}' already exists.", true);
        }

        if (await _context.Students.AnyAsync(s => s.Email.ToLower() == dto.Email.Trim().ToLower()))
        {
            return (null, $"Student with email '{dto.Email}' already exists.", true);
        }

        var student = new Student
        {
            StudentNumber = dto.StudentNumber.Trim(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            Phone = dto.Phone?.Trim() ?? string.Empty,
            DateOfBirth = DateTime.SpecifyKind(dto.DateOfBirth, DateTimeKind.Utc),
            Department = dto.Department.Trim(),
            Semester = dto.Semester,
            CreatedAt = DateTime.UtcNow
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var result = new StudentDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Phone = student.Phone,
            DateOfBirth = student.DateOfBirth,
            Department = student.Department,
            Semester = student.Semester,
            CreatedAt = student.CreatedAt
        };

        return (result, null, false);
    }

    public async Task<(StudentDto? Student, string? ErrorMessage, bool IsConflict)> UpdateStudentAsync(int id, UpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return (null, $"Student with ID {id} was not found.", false);
        }

        // Email conflict check
        if (await _context.Students.AnyAsync(s => s.Id != id && s.Email.ToLower() == dto.Email.Trim().ToLower()))
        {
            return (null, $"Student with email '{dto.Email}' already exists.", true);
        }

        student.FirstName = dto.FirstName.Trim();
        student.LastName = dto.LastName.Trim();
        student.Email = dto.Email.Trim();
        student.Phone = dto.Phone?.Trim() ?? string.Empty;
        student.DateOfBirth = DateTime.SpecifyKind(dto.DateOfBirth, DateTimeKind.Utc);
        student.Department = dto.Department.Trim();
        student.Semester = dto.Semester;

        await _context.SaveChangesAsync();

        var result = new StudentDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Phone = student.Phone,
            DateOfBirth = student.DateOfBirth,
            Department = student.Department,
            Semester = student.Semester,
            CreatedAt = student.CreatedAt
        };

        return (result, null, false);
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}
