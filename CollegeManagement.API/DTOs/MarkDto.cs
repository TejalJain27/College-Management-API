using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs;

public class MarkDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public decimal MarksObtained { get; set; }
    public decimal MaximumMarks { get; set; }
    public decimal Percentage => MaximumMarks == 0 ? 0 : Math.Round((MarksObtained / MaximumMarks) * 100, 2);
    public string Grade { get; set; } = string.Empty;
}

public class CreateMarkDto
{
    [Required(ErrorMessage = "Student is required")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Course is required")]
    public int CourseId { get; set; }

    [Range(0, 1000, ErrorMessage = "Marks obtained must be non-negative")]
    public decimal MarksObtained { get; set; }

    [Range(1, 1000, ErrorMessage = "Maximum marks must be greater than zero")]
    public decimal MaximumMarks { get; set; }
}

public class UpdateMarkDto
{
    [Range(0, 1000)]
    public decimal MarksObtained { get; set; }

    [Range(1, 1000)]
    public decimal MaximumMarks { get; set; }
}
