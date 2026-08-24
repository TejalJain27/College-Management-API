using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string CourseName { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Credits { get; set; }

    [Required]
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Semester { get; set; }

    public int? FacultyId { get; set; }
    public Faculty? Faculty { get; set; }

    // Navigation properties
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
    public ICollection<Mark> Marks { get; set; } = new List<Mark>();
}
