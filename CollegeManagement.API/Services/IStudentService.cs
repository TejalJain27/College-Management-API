using CollegeManagement.API.DTOs;

namespace CollegeManagement.API.Services;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetStudentsAsync(string? search = null, string? department = null, int? semester = null);
    Task<StudentDto?> GetStudentByIdAsync(int id);
    Task<(StudentDto? Student, string? ErrorMessage, bool IsConflict)> CreateStudentAsync(CreateStudentDto dto);
    Task<(StudentDto? Student, string? ErrorMessage, bool IsConflict)> UpdateStudentAsync(int id, UpdateStudentDto dto);
    Task<bool> DeleteStudentAsync(int id);
}
