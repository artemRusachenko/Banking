namespace Banking.Domain.Transactions
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
    }
}
