using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Faculty> FacultyMembers => Set<Faculty>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Attendance> AttendanceRecords => Set<Attendance>();
    public DbSet<Mark> Marks => Set<Mark>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.StudentNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Faculty configuration
        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(e => e.CourseCode).IsUnique();

            entity.HasOne(c => c.Faculty)
                .WithMany(f => f.Courses)
                .HasForeignKey(c => c.FacultyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Enrollment configuration
        modelBuilder.Entity<Enrollment>(entity =>
        {
            // Unique composite index to prevent duplicate enrollments
            entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Attendance configuration
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasOne(a => a.Student)
                .WithMany(s => s.AttendanceRecords)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Course)
                .WithMany(c => c.AttendanceRecords)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Mark configuration
        modelBuilder.Entity<Mark>(entity =>
        {
            entity.HasOne(m => m.Student)
                .WithMany(s => s.Marks)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Course)
                .WithMany(c => c.Marks)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
