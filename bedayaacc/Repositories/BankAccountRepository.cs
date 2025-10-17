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

        public async Task<List<BankAccount>> GetActiveAccountsAsync()
        {
            const string sql = @"
SELECT * FROM dbo.BankAccounts
WHERE IsActive = 1
ORDER BY IsDefault DESC, DisplayOrder, BankAccountId;";
            using var c = Conn();
            var list = await c.QueryAsync<BankAccount>(sql);
            return list.ToList();
        }

        public async Task<BankAccount?> GetDefaultAccountAsync()
        {
            const string sql = @"
SELECT TOP 1 * FROM dbo.BankAccounts
WHERE IsActive = 1
ORDER BY IsDefault DESC, DisplayOrder, BankAccountId;";
            using var c = Conn();
            return await c.QueryFirstOrDefaultAsync<BankAccount>(sql);
        }
    }
}
