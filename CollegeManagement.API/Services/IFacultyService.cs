using CollegeManagement.API.DTOs;

namespace CollegeManagement.API.Services;

public interface IFacultyService
{
    Task<IEnumerable<FacultyDto>> GetFacultyAsync();
    Task<FacultyDto?> GetFacultyByIdAsync(int id);
    Task<(FacultyDto? Faculty, string? ErrorMessage, bool IsConflict)> CreateFacultyAsync(CreateFacultyDto dto);
    Task<(FacultyDto? Faculty, string? ErrorMessage, bool IsConflict)> UpdateFacultyAsync(int id, UpdateFacultyDto dto);
    Task<bool> DeleteFacultyAsync(int id);
}
