using bedayaacc.Models;
using System.Threading;
using System.Threading.Tasks;
using static bedayaacc.Components.Pages.Student.Profile;

namespace bedayaacc.Repositories
{
    public interface IStudentDashboardRepository
    {
        Task<StudentProfileDto?> GetProfileAsync(int userId, CancellationToken ct = default);
        Task<DashboardCountersDto?> GetCountersAsync(int userId, CancellationToken ct = default);
        Task<IReadOnlyList<RecentActivityItemDto>> GetRecentActivityAsync(int userId, int top = 10, CancellationToken ct = default);
        Task<IReadOnlyList<UpcomingExamDto>> GetUpcomingExamsAsync(int userId, int daysAhead = 30, CancellationToken ct = default);
        Task<IReadOnlyList<InProgressAttemptDto>> GetInProgressAsync(int userId, CancellationToken ct = default);
        Task<IReadOnlyList<PassedCertificateDto>> GetPassedCertificatesAsync(int userId, int top = 10, CancellationToken ct = default);
        Task<IReadOnlyList<ExamProgressItemDto>> GetExamProgressListAsync(int userId, CancellationToken ct = default);
        Task<PagedResult<RecentActivityItemDto>> GetRecentActivityPageAsync(int userId, int page, int pageSize, CancellationToken ct = default);

        Task<bool> UpdateProfileAsync(UpdateProfileDto dto, CancellationToken ct = default);



    }
}
