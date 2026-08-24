using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateEnrollmentDto
{
    [Required(ErrorMessage = "Student is required")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Course is required")]
    public int CourseId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Active";
}

public class UpdateEnrollmentDto
{
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Active";
}
