using bedayaacc.Models;
using System.Threading;
using System.Threading.Tasks;
using static bedayaacc.Components.Pages.Student.Profile;

namespace bedayaacc.Services
{
    public interface IStudentDashboardService
    {
        Task<StudentDashboardVm> GetDashboardAsync(int userId, int recentTop = 10, int certTop = 10, int upcomingDays = 30, CancellationToken ct = default);
        Task<PagedResult<RecentActivityItemDto>> GetRecentActivityPageAsync(int userId, int page, int pageSize);

        Task<bool> UpdateProfileAsync(UpdateProfileDto dto);

    }
}
