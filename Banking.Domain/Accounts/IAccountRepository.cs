namespace Banking.Domain.Accounts
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account);
        Task<Account?> GetByAccountNumberAsync(string accountNumber);
        Task<IEnumerable<Account>> GetAllAsync();
    }
}
