// Repositories/IBankAccountRepository.cs
using bedayaacc.Models;

namespace bedayaacc.Repositories
{
    public interface IBankAccountRepository
    {
        Task<List<BankAccount>> GetActiveAccountsAsync();
        Task<BankAccount?> GetDefaultAccountAsync();
    }
}
