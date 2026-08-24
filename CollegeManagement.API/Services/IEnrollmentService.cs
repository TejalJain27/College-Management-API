using CollegeManagement.API.DTOs;

namespace CollegeManagement.API.Services;

public interface IEnrollmentService
{
    Task<IEnumerable<EnrollmentDto>> GetEnrollmentsAsync();
    Task<EnrollmentDto?> GetEnrollmentByIdAsync(int id);
    Task<(EnrollmentDto? Enrollment, string? ErrorMessage, bool IsConflict)> CreateEnrollmentAsync(CreateEnrollmentDto dto);
    Task<(EnrollmentDto? Enrollment, string? ErrorMessage, bool IsConflict)> UpdateEnrollmentAsync(int id, UpdateEnrollmentDto dto);
    Task<bool> DeleteEnrollmentAsync(int id);
}
