namespace Banking.Domain
{
    public class Transfer
    {
        public Guid Id { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }

        public virtual Account? FromAccount { get; set; }
        public virtual Account? ToAccount { get; set; }

        public Transfer(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            Id = Guid.NewGuid();
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            Amount = amount;
            TransferDate = DateTime.UtcNow;
        }
    }
}
