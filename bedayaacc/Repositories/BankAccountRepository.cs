// Repositories/BankAccountRepository.cs
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using bedayaacc.Models;

namespace bedayaacc.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly string _cs;
        public BankAccountRepository(IConfiguration cfg)
            => _cs = cfg.GetConnectionString("BedayaDB")
                  ?? cfg.GetConnectionString("DefaultConnection")
                  ?? throw new InvalidOperationException("Missing connection string 'BedayaDB'.");

        private SqlConnection Conn() => new SqlConnection(_cs);

        // --- Convenience (اختياري) ---
        public async Task<List<BankAccount>> GetActiveAccountsAsync()
        {
            const string sql = @"
SELECT *
FROM dbo.BankAccounts
WHERE IsActive = 1
ORDER BY IsDefault DESC, ISNULL(DisplayOrder, 2147483647), BankAccountId;";
            using var c = Conn();
            var list = await c.QueryAsync<BankAccount>(sql);
            return list.ToList();
        }

        public async Task<BankAccount?> GetDefaultAccountAsync()
        {
            const string sql = @"
SELECT TOP 1 *
FROM dbo.BankAccounts
WHERE IsActive = 1
ORDER BY IsDefault DESC, ISNULL(DisplayOrder, 2147483647), BankAccountId;";
            using var c = Conn();
            return await c.QueryFirstOrDefaultAsync<BankAccount>(sql);
        }

        // --- واجهة IBankAccountRepository ---

        public async Task<List<BankAccount>> GetAllAsync(string? search = null, bool? isActive = null)
        {
            // ملاحظة: أغلب الـ collations افتراضيًا case-insensitive، فمش محتاج Lower().
            const string sql = @"
SELECT *
FROM dbo.BankAccounts
WHERE (@search IS NULL
       OR BankName      LIKE '%' + @search + '%'
       OR AccountName   LIKE '%' + @search + '%'
       OR AccountNumber LIKE '%' + @search + '%'
       OR IBAN          LIKE '%' + @search + '%'
       OR SwiftCode     LIKE '%' + @search + '%'
       OR Branch        LIKE '%' + @search + '%'
       OR Currency      LIKE '%' + @search + '%')
  AND (@isActive IS NULL OR IsActive = @isActive)
ORDER BY IsDefault DESC, ISNULL(DisplayOrder, 2147483647), CreatedAt DESC;";

            using var c = Conn();
            var rows = await c.QueryAsync<BankAccount>(
                sql,
                new { search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(), isActive }
            );
            return rows.ToList();
        }

        public async Task<BankAccount?> GetByIdAsync(int id)
        {
            const string sql = @"SELECT TOP 1 * FROM dbo.BankAccounts WHERE BankAccountId = @id;";
            using var c = Conn();
            return await c.QueryFirstOrDefaultAsync<BankAccount>(sql, new { id });
        }

        public async Task<int> UpsertAsync(BankAccount model)
        {
            using var c = Conn();
            await c.OpenAsync();

            if (model.BankAccountId == 0)
            {
                const string insertSql = @"
INSERT INTO dbo.BankAccounts
    (BankName, AccountName, AccountNumber, IBAN, SwiftCode, Branch, Currency,
     IsActive, IsDefault, DisplayOrder, Notes, CreatedAt, UpdatedAt)
VALUES
    (@BankName, @AccountName, @AccountNumber, @IBAN, @SwiftCode, @Branch, @Currency,
     @IsActive, @IsDefault, @DisplayOrder, @Notes, SYSUTCDATETIME(), SYSUTCDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS int);";

                // تقدر تخلي IsDefault دائمًا 0 هنا لو حابب تجبر التعيين عبر SetDefaultAsync
                var newId = await c.ExecuteScalarAsync<int>(insertSql, new
                {
                    model.BankName,
                    model.AccountName,
                    model.AccountNumber,
                    model.IBAN,
                    model.SwiftCode,
                    model.Branch,
                    model.Currency,
                    model.IsActive,
                    model.IsDefault,     // أو استخدم false لو عايز تمنع التعيين مباشرة
                    model.DisplayOrder,
                    model.Notes
                });
                return newId;
            }
            else
            {
                const string updateSql = @"
UPDATE dbo.BankAccounts
SET BankName     = @BankName,
    AccountName  = @AccountName,
    AccountNumber= @AccountNumber,
    IBAN         = @IBAN,
    SwiftCode    = @SwiftCode,
    Branch       = @Branch,
    Currency     = @Currency,
    IsActive     = @IsActive,
    -- لا نعدل IsDefault هنا؛ استخدم SetDefaultAsync لضمان واحد فقط افتراضي
    DisplayOrder = @DisplayOrder,
    Notes        = @Notes,
    UpdatedAt    = SYSUTCDATETIME()
WHERE BankAccountId = @BankAccountId;";

                await c.ExecuteAsync(updateSql, new
                {
                    model.BankName,
                    model.AccountName,
                    model.AccountNumber,
                    model.IBAN,
                    model.SwiftCode,
                    model.Branch,
                    model.Currency,
                    model.IsActive,
                    model.DisplayOrder,
                    model.Notes,
                    model.BankAccountId
                });
                return model.BankAccountId;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"DELETE FROM dbo.BankAccounts WHERE BankAccountId = @id;";
            using var c = Conn();
            var affected = await c.ExecuteAsync(sql, new { id });
            return affected > 0;
        }

        public async Task<bool> SetDefaultAsync(int id)
        {
            using var c = Conn();
            await c.OpenAsync();
            using var tx = c.BeginTransaction();

            try
            {
                // لو الحسابات خاصة بكل محاضر، أضف شرط InstructorId في التحديثين
                const string clearSql = @"UPDATE dbo.BankAccounts SET IsDefault = 0;";
                const string setSql = @"UPDATE dbo.BankAccounts SET IsDefault = 1, UpdatedAt = SYSUTCDATETIME() WHERE BankAccountId = @id;";

                await c.ExecuteAsync(clearSql, transaction: tx);
                var affected = await c.ExecuteAsync(setSql, new { id }, tx);

                tx.Commit();
                return affected > 0;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
