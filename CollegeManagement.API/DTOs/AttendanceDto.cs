using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs;

public class AttendanceDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsPresent { get; set; }
}

public class CreateAttendanceDto
{
    [Required(ErrorMessage = "Student is required")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Course is required")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }

    public bool IsPresent { get; set; }
}

public class UpdateAttendanceDto
{
    [Required]
    public DateTime Date { get; set; }

    public bool IsPresent { get; set; }
}

public class AttendanceSummaryDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public int TotalClasses { get; set; }
    public int PresentClasses { get; set; }
    public double Percentage => TotalClasses == 0 ? 0 : Math.Round((double)PresentClasses / TotalClasses * 100, 2);
}
