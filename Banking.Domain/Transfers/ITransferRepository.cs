namespace Banking.Domain.Transfers
{
    public interface ITransferRepository
    {
        Task AddAsync(Transfer transfer);
    }
}
