using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.Models;

public class Faculty
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string EmployeeNumber { get; set; } = string.Empty;

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

    [Required]
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Designation { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
