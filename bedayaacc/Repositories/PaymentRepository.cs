using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using bedayaacc.Models;

namespace bedayaacc.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string _connString;
        public PaymentRepository(IConfiguration cfg)
            => _connString = cfg.GetConnectionString("BedayaDB")
                  ?? cfg.GetConnectionString("DefaultConnection")
                  ?? throw new InvalidOperationException("Missing connection string 'BedayaDB'.");


        private SqlConnection GetConn() => new SqlConnection(_connString);

        public async Task<int> CreateOrderAsync(int userId, int examId, string currency = "EGP", decimal discountAmount = 0)
        {
            const string sql = @"
INSERT INTO dbo.ExamOrders (UserId, ExamId, Currency, PriceAtPurchase, DiscountAmount, PaidAmount, Status, CreatedAt, IsDeleted, PaymentMethod)
SELECT @UserId, @ExamId, @Currency, e.Price, @DiscountAmount, 0, N'PENDING', SYSUTCDATETIME(), 0, N'BANK_TRANSFER'
FROM dbo.Exams e
WHERE e.ExamId = @ExamId AND e.IsDeleted = 0;

SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = GetConn();
            return await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId, ExamId = examId, Currency = currency, DiscountAmount = discountAmount });
        }

        public async Task<int> CreateBankTransferOrderAsync(int userId, int examId, string currency = "EGP", decimal discountAmount = 0)
            => await CreateOrderAsync(userId, examId, currency, discountAmount);

        public async Task<bool> UploadReceiptAndMarkPaidUnverifiedAsync(int orderId, decimal paidAmount, string receiptFileName, string receiptUrl, string? bankRef)
        {
            const string sql = @"
UPDATE dbo.ExamOrders
SET Status = N'PAID_UNVERIFIED',
    PaidAmount = @PaidAmount,
    ReceiptFileName = @ReceiptFileName,
    ReceiptUrl = @ReceiptUrl,
    ReceiptUploadedAt = SYSUTCDATETIME(),
    BankRef = @BankRef,
    PaidAt = SYSUTCDATETIME()
WHERE OrderId = @OrderId AND IsDeleted = 0;";

            using var conn = GetConn();
            var rows = await conn.ExecuteAsync(sql, new { OrderId = orderId, PaidAmount = paidAmount, ReceiptFileName = receiptFileName, ReceiptUrl = receiptUrl, BankRef = bankRef });
            return rows > 0;
        }

        public async Task<List<int>> GetPaidExamIdsAsync(int userId)
        {
            const string sql = @"
SELECT DISTINCT ExamId
FROM dbo.ExamOrders
WHERE UserId=@UserId AND Status IN (N'PAID', N'PAID_UNVERIFIED') AND IsDeleted=0;";
            using var conn = GetConn();
            var list = await conn.QueryAsync<int>(sql, new { UserId = userId });
            return list.ToList();
        }

        public async Task<ExamOrder?> GetOrderAsync(int orderId, int userId)
        {
            const string sql = @"SELECT * FROM dbo.ExamOrders WHERE OrderId=@OrderId AND UserId=@UserId AND IsDeleted=0;";
            using var conn = GetConn();
            return await conn.QueryFirstOrDefaultAsync<ExamOrder>(sql, new { OrderId = orderId, UserId = userId });
        }

        public async Task<ExamOrder?> GetLastPaidOrderAsync(int userId, int examId)
        {
            const string sql = @"
SELECT TOP 1 * FROM dbo.ExamOrders
WHERE UserId=@UserId AND ExamId=@ExamId AND Status IN (N'PAID', N'PAID_UNVERIFIED') AND IsDeleted=0
ORDER BY PaidAt DESC;";
            using var conn = GetConn();
            return await conn.QueryFirstOrDefaultAsync<ExamOrder>(sql, new { UserId = userId, ExamId = examId });
        }

        public async Task<bool> HasUserPaidExamAsync(int examId, int userId)
        {
            const string sql = @"
SELECT CASE WHEN EXISTS(
   SELECT 1 FROM dbo.ExamOrders
   WHERE ExamId=@ExamId AND UserId=@UserId
     AND Status IN (N'PAID') AND IsDeleted=0
) THEN 1 ELSE 0 END;";
            using var conn = GetConn();
            var v = await conn.ExecuteScalarAsync<int>(sql, new { ExamId = examId, UserId = userId });
            return v == 1;
        }

        public async Task<ExamOrder?> GetPendingOrderAsync(int userId, int examId)
        {
            const string sql = @"
SELECT TOP 1 *
FROM dbo.ExamOrders
WHERE UserId=@UserId AND ExamId=@ExamId AND IsDeleted=0
  AND Status IN (N'PENDING', N'FAILED', N'CANCELLED')
ORDER BY CreatedAt DESC;";
            using var conn = GetConn();
            return await conn.QueryFirstOrDefaultAsync<ExamOrder>(sql, new { UserId = userId, ExamId = examId });
        }


        // =========================
        //  A) قائمة الطلبات للمدرّس
        // =========================
        public async Task<(List<OrderReviewItem> Items, int Total)> GetOrdersForInstructorAsync(
            int instructorId, string? status, int page, int pageSize, string? search)
        {
            const string sqlCount = @"
SELECT COUNT(1)
FROM dbo.ExamOrders o
JOIN dbo.Exams e 
    ON e.ExamId = o.ExamId 
   AND e.IsDeleted = 0 
   AND e.InstructorId = @InstructorId
LEFT JOIN dbo.Users u ON u.UserId = o.UserId
WHERE o.IsDeleted = 0
  AND (@Status IS NULL OR @Status = '' OR o.Status = @Status)
  AND (@Search IS NULL OR @Search = '' OR
       (LTRIM(RTRIM(COALESCE(u.FirstName,''))) + ' ' + LTRIM(RTRIM(COALESCE(u.LastName,'')))) LIKE '%' + @Search + '%'
       OR u.Email LIKE '%' + @Search + '%'
       OR e.ExamTitleAr LIKE '%' + @Search + '%'
       OR e.ExamTitleEn LIKE '%' + @Search + '%');";

            const string sqlPage = @"
SELECT 
    o.OrderId,
    o.UserId,
    (LTRIM(RTRIM(COALESCE(u.FirstName,''))) + ' ' + LTRIM(RTRIM(COALESCE(u.LastName,'')))) AS UserName,
    u.Email AS UserEmail,
    o.ExamId,
    COALESCE(NULLIF(e.ExamTitleAr,''), e.ExamTitleEn) AS ExamTitle,
    o.PaidAmount,
    COALESCE(o.Currency,'EGP') AS Currency,
    o.Status,
    o.ReceiptUrl,
    o.BankRef,
    o.CreatedAt,
    o.PaidAt
FROM dbo.ExamOrders o
JOIN dbo.Exams e 
    ON e.ExamId = o.ExamId 
   AND e.IsDeleted = 0 
   AND e.InstructorId = @InstructorId
LEFT JOIN dbo.Users u ON u.UserId = o.UserId
WHERE o.IsDeleted = 0
  AND (@Status IS NULL OR @Status = '' OR o.Status = @Status)
  AND (@Search IS NULL OR @Search = '' OR
       (LTRIM(RTRIM(COALESCE(u.FirstName,''))) + ' ' + LTRIM(RTRIM(COALESCE(u.LastName,'')))) LIKE '%' + @Search + '%'
       OR u.Email LIKE '%' + @Search + '%'
       OR e.ExamTitleAr LIKE '%' + @Search + '%'
       OR e.ExamTitleEn LIKE '%' + @Search + '%')
ORDER BY COALESCE(o.PaidAt, o.CreatedAt) DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var offset = (page <= 1 ? 0 : (page - 1) * pageSize);

            using var c = GetConn();
            await c.OpenAsync();

            var total = await c.ExecuteScalarAsync<int>(sqlCount, new
            {
                InstructorId = instructorId,
                Status = status,
                Search = search
            });

            var items = (await c.QueryAsync<OrderReviewItem>(sqlPage, new
            {
                InstructorId = instructorId,
                Status = status,
                Search = search,
                Offset = offset,
                PageSize = pageSize
            })).ToList();

            return (items, total);
        }

        // =========================
        //  B) اعتماد الدفع
        // =========================
        public async Task<bool> MarkOrderVerifiedAsync(int orderId, int verifiedBy)
        {
            const string sql = @"
UPDATE dbo.ExamOrders
SET Status = N'PAID',
    VerifiedBy = @VerifiedBy,
    VerifiedAt = SYSUTCDATETIME(),
    UpdatedAt = SYSUTCDATETIME()
WHERE OrderId = @OrderId 
  AND IsDeleted = 0
  AND Status IN (N'PAID_UNVERIFIED', N'PENDING');";

            using var c = GetConn();
            var rows = await c.ExecuteAsync(sql, new { OrderId = orderId, VerifiedBy = verifiedBy });
            return rows > 0;
        }

        // =========================
        //  C) رفض الدفع
        // =========================
        public async Task<bool> MarkOrderRejectedAsync(int orderId, int verifiedBy, string? reason)
        {
            const string sql = @"
UPDATE dbo.ExamOrders
SET Status = N'FAILED',
    RejectReason = @Reason,
    VerifiedBy = @VerifiedBy,
    VerifiedAt = SYSUTCDATETIME(),
    UpdatedAt = SYSUTCDATETIME()
WHERE OrderId = @OrderId 
  AND IsDeleted = 0
  AND Status IN (N'PAID_UNVERIFIED', N'PENDING');";

            using var c = GetConn();
            var rows = await c.ExecuteAsync(sql, new { OrderId = orderId, VerifiedBy = verifiedBy, Reason = reason });
            return rows > 0;
        }
    }

}
