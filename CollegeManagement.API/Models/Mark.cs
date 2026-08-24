using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.Models;

public class Mark
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    [Range(0, 1000)]
    public decimal MarksObtained { get; set; }

    [Range(1, 1000)]
    public decimal MaximumMarks { get; set; }

    [Required]
    [StringLength(5)]
    public string Grade { get; set; } = string.Empty;
}
