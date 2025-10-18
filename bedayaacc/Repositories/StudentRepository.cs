using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using bedayaacc.Models;

namespace bedayaacc.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _cs;
        public StudentRepository(IConfiguration cfg)
            => _cs = cfg.GetConnectionString("BedayaDB")
                      ?? cfg.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Missing connection string 'BedayaDB'.");

        private SqlConnection Conn() => new SqlConnection(_cs);

        public async Task<PagedResult<StudentListItem>> GetStudentsAsync(
            string? search, int page, int pageSize, bool? isActive = null)
        {
            var p = new
            {
                search = string.IsNullOrWhiteSpace(search) ? null : search!.Trim(),
                offset = (page - 1) * pageSize,
                pageSize,
                isActive
            };

            // نجيب كل المستخدمين اللي ليهم دور Student، ونحسب لهم إجمالي امتحانات/محاولات، وآخر نشاط
            const string sql = @"
-- page
WITH stu AS (
    SELECT DISTINCT u.UserId, u.FirstName, u.LastName, u.Email, u.Phone, u.IsActive, u.IsDeleted, u.CreatedAt, u.LastLoginAt
    FROM dbo.Users u
    JOIN dbo.UserRoles ur ON ur.UserId = u.UserId
    JOIN dbo.Roles r      ON r.RoleId = ur.RoleId
    WHERE (r.RoleName = 'Student' OR r.RoleId = 3)
      AND ISNULL(u.IsDeleted, 0) = 0
),
agg AS (
    SELECT s.UserId, s.FirstName, s.LastName, s.Email, s.Phone, s.IsActive,
           COUNT(DISTINCT eo.ExamId)     AS ExamsCount,
           COUNT(DISTINCT ea.AttemptId)  AS AttemptsCount,
           MAX(COALESCE(ea.SubmittedAt, ea.StartedAt, eo.PaidAt, eo.CreatedAt, s.LastLoginAt, s.CreatedAt)) AS LastActivity
    FROM stu s
    LEFT JOIN dbo.ExamOrders   eo ON eo.UserId = s.UserId
    LEFT JOIN dbo.ExamAttempts ea ON ea.UserId = s.UserId
    GROUP BY s.UserId, s.FirstName, s.LastName, s.Email, s.Phone, s.IsActive, s.CreatedAt, s.LastLoginAt
),
filt AS (
    SELECT * FROM agg
    WHERE (@search IS NULL
           OR (COALESCE(FirstName,'') + ' ' + COALESCE(LastName,'')) LIKE '%' + @search + '%'
           OR Email LIKE '%' + @search + '%'
           OR Phone LIKE '%' + @search + '%')
      AND (@isActive IS NULL OR IsActive = @isActive)
)
SELECT UserId, FirstName, LastName, Email, Phone, ExamsCount, AttemptsCount, LastActivity
FROM filt
ORDER BY CASE WHEN LastActivity IS NULL THEN 1 ELSE 0 END, LastActivity DESC, UserId DESC
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

-- total
WITH stu AS (
    SELECT DISTINCT u.UserId, u.FirstName, u.LastName, u.Email, u.Phone, u.IsActive, u.IsDeleted, u.CreatedAt, u.LastLoginAt
    FROM dbo.Users u
    JOIN dbo.UserRoles ur ON ur.UserId = u.UserId
    JOIN dbo.Roles r      ON r.RoleId = ur.RoleId
    WHERE (r.RoleName = 'Student' OR r.RoleId = 3)
      AND ISNULL(u.IsDeleted, 0) = 0
),
agg AS (
    SELECT s.UserId, s.FirstName, s.LastName, s.Email, s.Phone, s.IsActive,
           COUNT(DISTINCT eo.ExamId)     AS ExamsCount,
           COUNT(DISTINCT ea.AttemptId)  AS AttemptsCount,
           MAX(COALESCE(ea.SubmittedAt, ea.StartedAt, eo.PaidAt, eo.CreatedAt, s.LastLoginAt, s.CreatedAt)) AS LastActivity
    FROM stu s
    LEFT JOIN dbo.ExamOrders   eo ON eo.UserId = s.UserId
    LEFT JOIN dbo.ExamAttempts ea ON ea.UserId = s.UserId
    GROUP BY s.UserId, s.FirstName, s.LastName, s.Email, s.Phone, s.IsActive, s.CreatedAt, s.LastLoginAt
)
SELECT COUNT(*)
FROM agg
WHERE (@search IS NULL
       OR (COALESCE(FirstName,'') + ' ' + COALESCE(LastName,'')) LIKE '%' + @search + '%'
       OR Email LIKE '%' + @search + '%'
       OR Phone LIKE '%' + @search + '%')
  AND (@isActive IS NULL OR IsActive = @isActive);";

            using var c = Conn();
            using var multi = await c.QueryMultipleAsync(sql, p);
            var items = (await multi.ReadAsync<StudentListItem>()).ToList();
            var total = await multi.ReadSingleAsync<int>();

            return new PagedResult<StudentListItem>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<StudentDetailsDto?> GetStudentDetailsAsync(int studentUserId)
        {
            const string sql = @"
-- user
SELECT TOP 1 u.UserId,
       RTRIM(LTRIM(COALESCE(u.FirstName,'') + ' ' + COALESCE(u.LastName,''))) AS FullName,
       u.Email, u.Phone
FROM dbo.Users u
WHERE u.UserId = @studentUserId AND ISNULL(u.IsDeleted,0) = 0;

-- orders (كل المنصّة)
SELECT o.OrderId, o.ExamId,
       COALESCE(e.ExamTitleAr, e.ExamTitleEn) AS ExamTitle,
       o.Currency, o.PaidAmount, o.Status, o.CreatedAt, o.PaidAt
FROM dbo.ExamOrders o
JOIN dbo.Exams e ON e.ExamId = o.ExamId
WHERE o.UserId = @studentUserId
ORDER BY ISNULL(o.PaidAt, o.CreatedAt) DESC;

-- attempts (كل المنصّة)
SELECT a.AttemptId, a.ExamId,
       COALESCE(e.ExamTitleAr, e.ExamTitleEn) AS ExamTitle,
       a.StartedAt, a.SubmittedAt, a.Percentage, a.IsPassed, a.Status
FROM dbo.ExamAttempts a
JOIN dbo.Exams e ON e.ExamId = a.ExamId
WHERE a.UserId = @studentUserId
ORDER BY ISNULL(a.SubmittedAt, a.StartedAt) DESC;";

            using var c = Conn();
            using var multi = await c.QueryMultipleAsync(sql, new { studentUserId });

            var user = await multi.ReadFirstOrDefaultAsync<(int UserId, string? FullName, string? Email, string? Phone)>();
            if (user.UserId == 0) return null;

            var orders = (await multi.ReadAsync<StudentOrderDto>()).ToList();
            var attempts = (await multi.ReadAsync<StudentAttemptDto>()).ToList();

            return new StudentDetailsDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Orders = orders,
                Attempts = attempts
            };
        }
    }
}
