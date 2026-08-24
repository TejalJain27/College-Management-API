using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs;
using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services;

public class FacultyService : IFacultyService
{
    private readonly ApplicationDbContext _context;

    public FacultyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FacultyDto>> GetFacultyAsync()
    {
        return await _context.FacultyMembers.AsNoTracking()
            .Select(f => new FacultyDto
            {
                Id = f.Id,
                EmployeeNumber = f.EmployeeNumber,
                FirstName = f.FirstName,
                LastName = f.LastName,
                Email = f.Email,
                Department = f.Department,
                Designation = f.Designation,
                CourseCount = f.Courses.Count
            }).ToListAsync();
    }

    public async Task<FacultyDto?> GetFacultyByIdAsync(int id)
    {
        var f = await _context.FacultyMembers.AsNoTracking()
            .Include(x => x.Courses)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (f == null) return null;

        return new FacultyDto
        {
            Id = f.Id,
            EmployeeNumber = f.EmployeeNumber,
            FirstName = f.FirstName,
            LastName = f.LastName,
            Email = f.Email,
            Department = f.Department,
            Designation = f.Designation,
            CourseCount = f.Courses.Count
        };
    }

    public async Task<(FacultyDto? Faculty, string? ErrorMessage, bool IsConflict)> CreateFacultyAsync(CreateFacultyDto dto)
    {
        if (await _context.FacultyMembers.AnyAsync(f => f.EmployeeNumber.ToLower() == dto.EmployeeNumber.Trim().ToLower()))
        {
            return (null, $"Faculty with employee number '{dto.EmployeeNumber}' already exists.", true);
        }

        if (await _context.FacultyMembers.AnyAsync(f => f.Email.ToLower() == dto.Email.Trim().ToLower()))
        {
            return (null, $"Faculty with email '{dto.Email}' already exists.", true);
        }

        var faculty = new Faculty
        {
            EmployeeNumber = dto.EmployeeNumber.Trim(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            Department = dto.Department.Trim(),
            Designation = dto.Designation.Trim()
        };

        _context.FacultyMembers.Add(faculty);
        await _context.SaveChangesAsync();

        var result = new FacultyDto
        {
            Id = faculty.Id,
            EmployeeNumber = faculty.EmployeeNumber,
            FirstName = faculty.FirstName,
            LastName = faculty.LastName,
            Email = faculty.Email,
            Department = faculty.Department,
            Designation = faculty.Designation,
            CourseCount = 0
        };

        return (result, null, false);
    }

    public async Task<(FacultyDto? Faculty, string? ErrorMessage, bool IsConflict)> UpdateFacultyAsync(int id, UpdateFacultyDto dto)
    {
        var faculty = await _context.FacultyMembers.FindAsync(id);
        if (faculty == null) return (null, $"Faculty with ID {id} not found.", false);

        if (await _context.FacultyMembers.AnyAsync(f => f.Id != id && f.Email.ToLower() == dto.Email.Trim().ToLower()))
        {
            return (null, $"Faculty with email '{dto.Email}' already exists.", true);
        }

        faculty.FirstName = dto.FirstName.Trim();
        faculty.LastName = dto.LastName.Trim();
        faculty.Email = dto.Email.Trim();
        faculty.Department = dto.Department.Trim();
        faculty.Designation = dto.Designation.Trim();

        await _context.SaveChangesAsync();

        var courseCount = await _context.Courses.CountAsync(c => c.FacultyId == id);

        var result = new FacultyDto
        {
            Id = faculty.Id,
            EmployeeNumber = faculty.EmployeeNumber,
            FirstName = faculty.FirstName,
            LastName = faculty.LastName,
            Email = faculty.Email,
            Department = faculty.Department,
            Designation = faculty.Designation,
            CourseCount = courseCount
        };

        return (result, null, false);
    }

    public async Task<bool> DeleteFacultyAsync(int id)
    {
        var faculty = await _context.FacultyMembers.FindAsync(id);
        if (faculty == null) return false;

        _context.FacultyMembers.Remove(faculty);
        await _context.SaveChangesAsync();
        return true;
    }
}
