using bedayaacc.Models;

namespace bedayaacc.Repositories
{
    public interface IStudentRepository
    {
        Task<PagedResult<StudentListItem>> GetStudentsAsync(
         string? search, int page, int pageSize, bool? isActive = null);

        Task<StudentDetailsDto?> GetStudentDetailsAsync(int studentUserId);
    }
}
