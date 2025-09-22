using bedayaacc.Models;

namespace bedayaacc.Services.Repositories
{
    public interface ICourseRepository
    {
        Task<PagedResult<CourseDto>> GetAsync(CourseFilter filter, string lang);
        Task<IReadOnlyList<CourseDto>> GetLatestAsync(int take, string lang);
        Task<CourseDetailsDto?> GetByIdAsync(int id, string lang);
        Task<IReadOnlyList<CourseDto>> GetRelatedAsync(int courseId, int take, string lang);       // لعرض ذات صلة
        Task<IReadOnlyList<CourseTrack>> GetTracksAsync(string lang);                               // فهرس المسارات
        Task<PagedResult<CourseDto>> GetByTrackAsync(string trackSlug, CourseFilter filter, string lang); // كورسات المسار مع فلترة
        Task<HashSet<string>> GetAllTagsAsync();                                                    // كل الوسوم (للـUI)

    }
}
