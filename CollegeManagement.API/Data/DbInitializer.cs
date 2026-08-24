using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Apply pending EF Core Migrations automatically (creates tables & history tracking)
        context.Database.Migrate();

        // Check if database is already seeded
        if (context.Students.Any())
        {
            return; // DB has been seeded
        }

        // 1. Seed Faculty Members
        var facultyMembers = new Faculty[]
        {
            new Faculty { EmployeeNumber = "FAC001", FirstName = "Dr. Alan", LastName = "Turing", Email = "alan.turing@college.edu", Department = "Computer Science", Designation = "Professor" },
            new Faculty { EmployeeNumber = "FAC002", FirstName = "Grace", LastName = "Hopper", Email = "grace.hopper@college.edu", Department = "Computer Science", Designation = "Associate Professor" },
            new Faculty { EmployeeNumber = "FAC003", FirstName = "Katherine", LastName = "Johnson", Email = "katherine.j@college.edu", Department = "Mathematics", Designation = "Professor" },
            new Faculty { EmployeeNumber = "FAC004", FirstName = "Nikola", LastName = "Tesla", Email = "nikola.tesla@college.edu", Department = "Electrical Engineering", Designation = "Assistant Professor" },
            new Faculty { EmployeeNumber = "FAC005", FirstName = "Marie", LastName = "Curie", Email = "marie.curie@college.edu", Department = "Physics", Designation = "Professor" }
        };
        context.FacultyMembers.AddRange(facultyMembers);
        context.SaveChanges();

        // 2. Seed Courses
        var courses = new Course[]
        {
            new Course { CourseCode = "CS101", CourseName = "Introduction to Computer Science", Credits = 4, Department = "Computer Science", Semester = 1, FacultyId = facultyMembers[0].Id },
            new Course { CourseCode = "CS201", CourseName = "Data Structures & Algorithms", Credits = 4, Department = "Computer Science", Semester = 3, FacultyId = facultyMembers[0].Id },
            new Course { CourseCode = "CS301", CourseName = "Database Management Systems", Credits = 3, Department = "Computer Science", Semester = 5, FacultyId = facultyMembers[1].Id },
            new Course { CourseCode = "MATH101", CourseName = "Calculus & Linear Algebra", Credits = 4, Department = "Mathematics", Semester = 1, FacultyId = facultyMembers[2].Id },
            new Course { CourseCode = "MATH202", CourseName = "Discrete Mathematics", Credits = 3, Department = "Mathematics", Semester = 3, FacultyId = facultyMembers[2].Id },
            new Course { CourseCode = "EE101", CourseName = "Basic Electrical Engineering", Credits = 4, Department = "Electrical Engineering", Semester = 1, FacultyId = facultyMembers[3].Id },
            new Course { CourseCode = "PHY101", CourseName = "Engineering Physics", Credits = 3, Department = "Physics", Semester = 1, FacultyId = facultyMembers[4].Id },
            new Course { CourseCode = "CS401", CourseName = "Web Development & APIs", Credits = 3, Department = "Computer Science", Semester = 5, FacultyId = facultyMembers[1].Id }
        };
        context.Courses.AddRange(courses);
        context.SaveChanges();

        // 3. Seed Students
        var students = new Student[]
        {
            new Student { StudentNumber = "STU202401", FirstName = "Rahul", LastName = "Sharma", Email = "rahul.sharma@student.edu", Phone = "+1-555-0101", DateOfBirth = new DateTime(2003, 5, 12, 0, 0, 0, DateTimeKind.Utc), Department = "Computer Science", Semester = 5, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202402", FirstName = "Priya", LastName = "Patel", Email = "priya.patel@student.edu", Phone = "+1-555-0102", DateOfBirth = new DateTime(2004, 3, 24, 0, 0, 0, DateTimeKind.Utc), Department = "Computer Science", Semester = 3, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202403", FirstName = "Aarav", LastName = "Verma", Email = "aarav.verma@student.edu", Phone = "+1-555-0103", DateOfBirth = new DateTime(2005, 8, 15, 0, 0, 0, DateTimeKind.Utc), Department = "Mathematics", Semester = 1, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202404", FirstName = "Ananya", LastName = "Gupta", Email = "ananya.gupta@student.edu", Phone = "+1-555-0104", DateOfBirth = new DateTime(2003, 11, 30, 0, 0, 0, DateTimeKind.Utc), Department = "Computer Science", Semester = 5, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202405", FirstName = "Rohan", LastName = "Mehta", Email = "rohan.mehta@student.edu", Phone = "+1-555-0105", DateOfBirth = new DateTime(2004, 1, 10, 0, 0, 0, DateTimeKind.Utc), Department = "Electrical Engineering", Semester = 1, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202406", FirstName = "Sneha", LastName = "Reddy", Email = "sneha.reddy@student.edu", Phone = "+1-555-0106", DateOfBirth = new DateTime(2005, 4, 18, 0, 0, 0, DateTimeKind.Utc), Department = "Physics", Semester = 1, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202407", FirstName = "Vikram", LastName = "Singh", Email = "vikram.singh@student.edu", Phone = "+1-555-0107", DateOfBirth = new DateTime(2004, 9, 5, 0, 0, 0, DateTimeKind.Utc), Department = "Computer Science", Semester = 3, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202408", FirstName = "Kavya", LastName = "Nair", Email = "kavya.nair@student.edu", Phone = "+1-555-0108", DateOfBirth = new DateTime(2003, 7, 22, 0, 0, 0, DateTimeKind.Utc), Department = "Mathematics", Semester = 3, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202409", FirstName = "Dev", LastName = "Joshi", Email = "dev.joshi@student.edu", Phone = "+1-555-0109", DateOfBirth = new DateTime(2005, 2, 14, 0, 0, 0, DateTimeKind.Utc), Department = "Electrical Engineering", Semester = 1, CreatedAt = DateTime.UtcNow },
            new Student { StudentNumber = "STU202410", FirstName = "Ishita", LastName = "Deshmukh", Email = "ishita.d@student.edu", Phone = "+1-555-0110", DateOfBirth = new DateTime(2003, 12, 1, 0, 0, 0, DateTimeKind.Utc), Department = "Computer Science", Semester = 5, CreatedAt = DateTime.UtcNow }
        };
        context.Students.AddRange(students);
        context.SaveChanges();

        // 4. Seed Enrollments
        var enrollments = new Enrollment[]
        {
            new Enrollment { StudentId = students[0].Id, CourseId = courses[2].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-3) },
            new Enrollment { StudentId = students[0].Id, CourseId = courses[7].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-3) },
            new Enrollment { StudentId = students[1].Id, CourseId = courses[1].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-3) },
            new Enrollment { StudentId = students[1].Id, CourseId = courses[4].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-3) },
            new Enrollment { StudentId = students[2].Id, CourseId = courses[3].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-2) },
            new Enrollment { StudentId = students[3].Id, CourseId = courses[2].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-3) },
            new Enrollment { StudentId = students[4].Id, CourseId = courses[5].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-2) },
            new Enrollment { StudentId = students[5].Id, CourseId = courses[6].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-2) },
            new Enrollment { StudentId = students[6].Id, CourseId = courses[1].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-3) },
            new Enrollment { StudentId = students[9].Id, CourseId = courses[7].Id, Status = "Active", EnrollmentDate = DateTime.UtcNow.AddMonths(-3) }
        };
        context.Enrollments.AddRange(enrollments);
        context.SaveChanges();

        // 5. Seed Attendance
        var attendanceList = new List<Attendance>();
        var baseDate = DateTime.UtcNow.Date.AddDays(-10);

        for (int i = 0; i < 7; i++)
        {
            var date = baseDate.AddDays(i);
            attendanceList.Add(new Attendance { StudentId = students[0].Id, CourseId = courses[2].Id, Date = date, IsPresent = i % 6 != 0 });
            attendanceList.Add(new Attendance { StudentId = students[0].Id, CourseId = courses[7].Id, Date = date, IsPresent = true });
            attendanceList.Add(new Attendance { StudentId = students[1].Id, CourseId = courses[1].Id, Date = date, IsPresent = i % 4 != 0 });
            attendanceList.Add(new Attendance { StudentId = students[2].Id, CourseId = courses[3].Id, Date = date, IsPresent = true });
            attendanceList.Add(new Attendance { StudentId = students[3].Id, CourseId = courses[2].Id, Date = date, IsPresent = i % 5 != 0 });
        }
        context.AttendanceRecords.AddRange(attendanceList);
        context.SaveChanges();

        // 6. Seed Marks
        var marks = new Mark[]
        {
            new Mark { StudentId = students[0].Id, CourseId = courses[2].Id, MarksObtained = 88, MaximumMarks = 100, Grade = "A" },
            new Mark { StudentId = students[0].Id, CourseId = courses[7].Id, MarksObtained = 94, MaximumMarks = 100, Grade = "A+" },
            new Mark { StudentId = students[1].Id, CourseId = courses[1].Id, MarksObtained = 76, MaximumMarks = 100, Grade = "B" },
            new Mark { StudentId = students[1].Id, CourseId = courses[4].Id, MarksObtained = 82, MaximumMarks = 100, Grade = "A" },
            new Mark { StudentId = students[2].Id, CourseId = courses[3].Id, MarksObtained = 90, MaximumMarks = 100, Grade = "A+" },
            new Mark { StudentId = students[3].Id, CourseId = courses[2].Id, MarksObtained = 85, MaximumMarks = 100, Grade = "A" },
            new Mark { StudentId = students[4].Id, CourseId = courses[5].Id, MarksObtained = 68, MaximumMarks = 100, Grade = "B" },
            new Mark { StudentId = students[5].Id, CourseId = courses[6].Id, MarksObtained = 72, MaximumMarks = 100, Grade = "B" },
            new Mark { StudentId = students[6].Id, CourseId = courses[1].Id, MarksObtained = 60, MaximumMarks = 100, Grade = "C" },
            new Mark { StudentId = students[9].Id, CourseId = courses[7].Id, MarksObtained = 91, MaximumMarks = 100, Grade = "A+" }
        };
        context.Marks.AddRange(marks);
        context.SaveChanges();
    }
}
