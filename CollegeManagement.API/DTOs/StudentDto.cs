using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Department { get; set; } = string.Empty;
    public int Semester { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateStudentDto
{
    [Required(ErrorMessage = "Student number is required")]
    [StringLength(50)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Department is required")]
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Semester must be between 1 and 10")]
    public int Semester { get; set; }
}

public class UpdateStudentDto
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

    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Semester { get; set; }
}
