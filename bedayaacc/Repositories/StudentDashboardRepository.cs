using bedayaacc.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using static bedayaacc.Components.Pages.Student.Profile;

namespace bedayaacc.Repositories
{
    public sealed class StudentDashboardRepository : IStudentDashboardRepository
    {
        private readonly string _cs;

        public StudentDashboardRepository(IConfiguration cfg)
        {
            // اسم سلسلة الاتصال — ضعه في appsettings.json
            // "ConnectionStrings": { "BedayaDB": "Server=...;Database=BedayaDB;..." }
            _cs = cfg.GetConnectionString("BedayaDB")
                  ?? cfg.GetConnectionString("DefaultConnection")
                  ?? throw new InvalidOperationException("Missing connection string 'BedayaDB'.");
        }

        private SqlConnection Create() => new SqlConnection(_cs);

        public async Task<StudentProfileDto?> GetProfileAsync(int userId, CancellationToken ct = default)
        {
            using var cn = Create();
            return await cn.QueryFirstOrDefaultAsync<StudentProfileDto>(
                new CommandDefinition(
                    commandText: "dbo.sp_Student_Profile_Get",
                    parameters: new { UserId = userId },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));
        }

        public async Task<DashboardCountersDto?> GetCountersAsync(int userId, CancellationToken ct = default)
        {
            using var cn = Create();
            return await cn.QueryFirstOrDefaultAsync<DashboardCountersDto>(
                new CommandDefinition(
                    "dbo.sp_Student_Dashboard_Counters",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));
        }

        public async Task<IReadOnlyList<RecentActivityItemDto>> GetRecentActivityAsync(int userId, int top = 10, CancellationToken ct = default)
        {
            using var cn = Create();
            var rows = await cn.QueryAsync<RecentActivityItemDto>(
                new CommandDefinition(
                    "dbo.sp_Student_Dashboard_RecentActivity",
                    new { UserId = userId, Top = top },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));
            return rows.AsList();
        }

        public async Task<IReadOnlyList<UpcomingExamDto>> GetUpcomingExamsAsync(int userId, int daysAhead = 30, CancellationToken ct = default)
        {
            using var cn = Create();
            var rows = await cn.QueryAsync<UpcomingExamDto>(
                new CommandDefinition(
                    "dbo.sp_Student_Dashboard_UpcomingExams",
                    new { UserId = userId, DaysAhead = daysAhead },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));
            return rows.AsList();
        }

        public async Task<IReadOnlyList<InProgressAttemptDto>> GetInProgressAsync(int userId, CancellationToken ct = default)
        {
            using var cn = Create();
            var rows = await cn.QueryAsync<InProgressAttemptDto>(
                new CommandDefinition(
                    "dbo.sp_Student_Dashboard_InProgress",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));
            return rows.AsList();
        }

        public async Task<IReadOnlyList<PassedCertificateDto>> GetPassedCertificatesAsync(int userId, int top = 10, CancellationToken ct = default)
        {
            using var cn = Create();
            var rows = await cn.QueryAsync<PassedCertificateDto>(
                new CommandDefinition(
                    "dbo.sp_Student_Dashboard_PassedCertificates",
                    new { UserId = userId, Top = top },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));
            return rows.AsList();
        }

        public async Task<IReadOnlyList<ExamProgressItemDto>> GetExamProgressListAsync(int userId, CancellationToken ct = default)
        {
            using var cn = Create();
            var rows = await cn.QueryAsync<ExamProgressItemDto>(
                new CommandDefinition(
                    "dbo.sp_Student_Dashboard_ExamProgressList",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));
            return rows.AsList();
        }


        public async Task<bool> UpdateProfileAsync(UpdateProfileDto dto, CancellationToken ct = default)
        {
            using var cn = Create();

            // نستخدم COALESCE علشان لو الباراميتر NULL نسيب القيمة القديمة زي ما هي
            // AcceptMarketing منطقياً bool — لو عايز "مايغيرهوش" لما يكون default، خليه nullable في DTO واستخدم COALESCE عليه برضه.
            var sql = @"
UPDATE [dbo].[Users]
SET
    FirstName      = COALESCE(@FirstName, FirstName),
    LastName       = COALESCE(@LastName, LastName),
    Email          = COALESCE(@Email, Email),
    Phone          = COALESCE(@Phone, Phone),
    Bio            = COALESCE(@Bio, Bio),
    DateOfBirth    = COALESCE(@DateOfBirth, DateOfBirth),
    Country        = COALESCE(@Country, Country),
    City           = COALESCE(@City, City),
    AcceptMarketing= COALESCE(@AcceptMarketing, AcceptMarketing),
    UpdatedAt      = SYSUTCDATETIME()
WHERE UserId = @UserId";

            // لو AcceptMarketing عندك غير nullable في الجدول، خل DTO.AcceptMarketing نوعه bool? وتبعت null لما عايز ما تغيّرش القيمة.
            var rows = await cn.ExecuteAsync(
                new CommandDefinition(
                    commandText: sql,
                    parameters: new
                    {
                        dto.UserId,
                        dto.FirstName,
                        dto.LastName,
                        dto.Email,
                        dto.Phone,
                        dto.Bio,
                        dto.DateOfBirth,
                        dto.Country,
                        dto.City,
                        // مهم: لو DTO عندك bool مش nullable وعايز تدعم "عدم التغيير"، خليها bool? في الموديل
                        AcceptMarketing = (bool?)dto.AcceptMarketing
                    },
                    commandType: CommandType.Text,
                    cancellationToken: ct
                ));

            return rows > 0;
        }

        public async Task<PagedResult<RecentActivityItemDto>> GetRecentActivityPageAsync(int userId, int page, int pageSize, CancellationToken ct = default)
        {
            using var cn = Create();

            // نرجّع نتيجتين: (1) البيانات، (2) Count
            var sql = @"
;WITH Base AS (
    SELECT 
        a.AttemptId,
        a.ExamId,
        e.ExamTitleAr,
        e.ExamTitleEn,
        a.StartedAt,
        a.SubmittedAt,
        a.Percentage,
        CASE 
            WHEN a.SubmittedAt IS NOT NULL THEN 'AttemptSubmitted'
            WHEN a.StartedAt  IS NOT NULL THEN 'AttemptStarted'
            ELSE 'AccessGranted'
        END AS EventType,
        COALESCE(a.SubmittedAt, a.StartedAt) AS EventAt
    FROM dbo.ExamAttempts a
    INNER JOIN dbo.Exams e ON e.ExamId = a.ExamId
    WHERE a.UserId = @UserId

    UNION ALL

    SELECT 
        NULL AS AttemptId,
        x.ExamId,
        e.ExamTitleAr,
        e.ExamTitleEn,
        NULL AS StartedAt,
        NULL AS SubmittedAt,
        NULL AS Percentage,
        'AccessGranted' AS EventType,
        x.AccessGrantedAt AS EventAt
    FROM dbo.ExamAccess x
    INNER JOIN dbo.Exams e ON e.ExamId = x.ExamId
    WHERE x.UserId = @UserId
)
-- الصفحة المطلوبة
SELECT *
FROM Base
ORDER BY EventAt DESC
OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

-- إجمالي السجلات (بدون مشاكل أسماء أعمدة)
SELECT
    (SELECT COUNT(*) FROM dbo.ExamAttempts a WHERE a.UserId = @UserId)
  + (SELECT COUNT(*) FROM dbo.ExamAccess  x WHERE x.UserId = @UserId)
  AS TotalCount;
";


            using (var multi = await cn.QueryMultipleAsync(new CommandDefinition(
       sql,
       new { UserId = userId, Page = page, PageSize = pageSize },
       commandType: CommandType.Text,
       cancellationToken: ct)))
            {
                var rows = (await multi.ReadAsync<RecentActivityItemDto>()).ToList();
                var total = await multi.ReadFirstAsync<int>();

                return new PagedResult<RecentActivityItemDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = total,
                    Items = rows
                };
            }




        }
    }
}
