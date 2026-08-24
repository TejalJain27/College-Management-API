using CollegeManagement.API.DTOs;

namespace CollegeManagement.API.Services;

public interface IMarkService
{
    Task<IEnumerable<MarkDto>> GetMarksAsync();
    Task<MarkDto?> GetMarkByIdAsync(int id);
    Task<(MarkDto? Mark, string? ErrorMessage)> CreateMarkAsync(CreateMarkDto dto);
    Task<(MarkDto? Mark, string? ErrorMessage)> UpdateMarkAsync(int id, UpdateMarkDto dto);
    Task<bool> DeleteMarkAsync(int id);
}
