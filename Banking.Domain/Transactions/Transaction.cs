using Banking.Domain.Accounts;
using System.Text.Json.Serialization;

namespace Banking.Domain.Transactions
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public virtual Account? Account { get; set; }

        public Transaction(Guid accountId, TransactionType transactionType, decimal amount, string? description)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            TransactionType = transactionType;
            Amount = amount;
            TransactionDate = DateTime.UtcNow;

            if (description == null)
                description = $"{TransactionType} of {Amount}$ on {TransactionDate:yyyy-MM-dd HH:mm:ss} UTC";

            Description = description;
        }
    }
}
