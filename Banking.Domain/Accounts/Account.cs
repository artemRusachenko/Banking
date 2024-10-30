using Banking.Domain.Transactions;
using Banking.Domain.Transfers;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Banking.Domain.Accounts
{
    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public string HolderName { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();

        [JsonIgnore]
        public virtual ICollection<Transfer> TransferFrom { get; } = new List<Transfer>();
        [JsonIgnore]
        public virtual ICollection<Transfer> TransferTo { get; } = new List<Transfer>();

        public Account(string holderName)
        {
            Id = Guid.NewGuid();
            AccountNumber = GenerateAccountNumber();
            HolderName = holderName;
            Balance = 1000;
            CreatedAt = DateTime.UtcNow;
        }

        private static string GenerateAccountNumber()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[16];
                rng.GetBytes(randomNumber);

                return string.Join("", randomNumber.Select(b => b % 10));
            }
        }
    }
}
