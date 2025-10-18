// Repositories/IBankAccountRepository.cs
using bedayaacc.Models;

namespace bedayaacc.Repositories
{
    public interface IBankAccountRepository
    {
        Task<List<BankAccount>> GetActiveAccountsAsync();
        Task<BankAccount?> GetDefaultAccountAsync();


        Task<List<BankAccount>> GetAllAsync(string? search = null, bool? isActive = null);
        Task<BankAccount?> GetByIdAsync(int id);
        Task<int> UpsertAsync(BankAccount model);   // insert أو update — يعيد Id
        Task<bool> DeleteAsync(int id);
        Task<bool> SetDefaultAsync(int id);
    }
}
