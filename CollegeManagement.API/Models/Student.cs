using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.Models;

public class Student
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Semester { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
    public ICollection<Mark> Marks { get; set; } = new List<Mark>();
}
