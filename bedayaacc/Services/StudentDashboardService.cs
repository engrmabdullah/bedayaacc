using bedayaacc.Models;
using bedayaacc.Repositories;
using static bedayaacc.Components.Pages.Student.Profile;

namespace bedayaacc.Services
{
    public sealed class StudentDashboardService : IStudentDashboardService
    {
        private readonly IStudentDashboardRepository _repo;

        public StudentDashboardService(IStudentDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<StudentDashboardVm> GetDashboardAsync(int userId, int recentTop = 10, int certTop = 10, int upcomingDays = 30, CancellationToken ct = default)
        {
            // موازاة بسيطة
            var profileTask = _repo.GetProfileAsync(userId, ct);
            var countersTask = _repo.GetCountersAsync(userId, ct);
            var recentTask = _repo.GetRecentActivityAsync(userId, recentTop, ct);
            var upcomingTask = _repo.GetUpcomingExamsAsync(userId, upcomingDays, ct);
            var progressTask = _repo.GetExamProgressListAsync(userId, ct);
            var runningTask = _repo.GetInProgressAsync(userId, ct);
            var passedTask = _repo.GetPassedCertificatesAsync(userId, certTop, ct);

            await Task.WhenAll(profileTask, countersTask, recentTask, upcomingTask, progressTask, runningTask, passedTask);

            return new StudentDashboardVm
            {
                Profile = profileTask.Result,
                Counters = countersTask.Result,
                RecentActivity = recentTask.Result.ToList(),
                UpcomingExams = upcomingTask.Result.ToList(),
                ExamProgress = progressTask.Result.ToList(),
                InProgress = runningTask.Result.ToList(),
                PassedCertificates = passedTask.Result.ToList()
            };
        }

        public Task<PagedResult<RecentActivityItemDto>> GetRecentActivityPageAsync(int userId, int page, int pageSize)
      => _repo.GetRecentActivityPageAsync(userId, page, pageSize);

        public Task<bool> UpdateProfileAsync(UpdateProfileDto dto)
            => _repo.UpdateProfileAsync(dto);
    }
}
