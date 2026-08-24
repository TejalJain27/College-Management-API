using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var totalStudents = await _context.Students.CountAsync();
        var totalCourses = await _context.Courses.CountAsync();
        var totalFaculty = await _context.FacultyMembers.CountAsync();
        var totalEnrollments = await _context.Enrollments.CountAsync();

        var totalAttendanceRecords = await _context.AttendanceRecords.CountAsync();
        var presentAttendanceRecords = await _context.AttendanceRecords.CountAsync(a => a.IsPresent);
        double attendanceRate = totalAttendanceRecords == 0 ? 0 : Math.Round((double)presentAttendanceRecords / totalAttendanceRecords * 100, 2);

        var recentStudents = await _context.Students.AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Take(5)
            .Select(s => new StudentDto
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

        var departments = await _context.Students.Select(s => s.Department)
            .Union(_context.Courses.Select(c => c.Department))
            .Union(_context.FacultyMembers.Select(f => f.Department))
            .Distinct()
            .ToListAsync();

        var deptStats = new List<DepartmentStatDto>();
        foreach (var dept in departments)
        {
            deptStats.Add(new DepartmentStatDto
            {
                Department = dept,
                StudentCount = await _context.Students.CountAsync(s => s.Department == dept),
                CourseCount = await _context.Courses.CountAsync(c => c.Department == dept),
                FacultyCount = await _context.FacultyMembers.CountAsync(f => f.Department == dept)
            });
        }

        return new DashboardSummaryDto
        {
            TotalStudents = totalStudents,
            TotalCourses = totalCourses,
            TotalFaculty = totalFaculty,
            TotalEnrollments = totalEnrollments,
            AverageAttendanceRate = attendanceRate,
            RecentStudents = recentStudents,
            DepartmentStats = deptStats
        };
    }
}
