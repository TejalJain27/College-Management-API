using CollegeManagement.API.DTOs;

namespace CollegeManagement.API.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
}
