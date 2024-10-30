using Banking.Domain.Accounts;
using System.Text.Json.Serialization;

namespace Banking.Domain.Transfers
{
    public class Transfer
    {
        public Guid Id { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }

        [JsonIgnore]
        public virtual Account? FromAccount { get; set; }
        [JsonIgnore]
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
