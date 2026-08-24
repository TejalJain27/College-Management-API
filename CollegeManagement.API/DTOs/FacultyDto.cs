using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs;

public class FacultyDto
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int CourseCount { get; set; }
}

public class CreateFacultyDto
{
    [Required(ErrorMessage = "Employee number is required")]
    [StringLength(50)]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required")]
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Designation is required")]
    [StringLength(50)]
    public string Designation { get; set; } = string.Empty;
}

public class UpdateFacultyDto
{
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
}
