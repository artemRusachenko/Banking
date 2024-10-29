namespace Banking.Domain.Accounts
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account);
        Task<Account?> GetByAccountNumberAsync(string accountNumber);
        Task<IEnumerable<Account>> GetAllAsync();
        Task Deposit(Account account, decimal amount);
        Task Withdraw(Account account, decimal amount);
        Task Transfer(Account fromAccount, Account toAccount, decimal amount);
    }
}
