using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Department { get; set; } = string.Empty;
    public int Semester { get; set; }
    public int? FacultyId { get; set; }
    public string? FacultyName { get; set; }
    public int EnrolledStudentsCount { get; set; }
}

public class CreateCourseDto
{
    [Required(ErrorMessage = "Course code is required")]
    [StringLength(20)]
    public string CourseCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course name is required")]
    [StringLength(100)]
    public string CourseName { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10")]
    public int Credits { get; set; }

    [Required(ErrorMessage = "Department is required")]
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Semester must be between 1 and 10")]
    public int Semester { get; set; }

    public int? FacultyId { get; set; }
}

public class UpdateCourseDto
{
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
}
