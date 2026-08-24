namespace CollegeManagement.API.DTOs;

public class DashboardSummaryDto
{
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int TotalFaculty { get; set; }
    public int TotalEnrollments { get; set; }
    public double AverageAttendanceRate { get; set; }
    public List<StudentDto> RecentStudents { get; set; } = new();
    public List<DepartmentStatDto> DepartmentStats { get; set; } = new();
}

public class DepartmentStatDto
{
    public string Department { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public int CourseCount { get; set; }
    public int FacultyCount { get; set; }
}
