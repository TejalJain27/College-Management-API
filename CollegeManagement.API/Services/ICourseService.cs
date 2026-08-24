using CollegeManagement.API.DTOs;

namespace CollegeManagement.API.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetCoursesAsync();
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<(CourseDto? Course, string? ErrorMessage, bool IsConflict)> CreateCourseAsync(CreateCourseDto dto);
    Task<(CourseDto? Course, string? ErrorMessage, bool IsConflict)> UpdateCourseAsync(int id, UpdateCourseDto dto);
    Task<bool> DeleteCourseAsync(int id);
}
