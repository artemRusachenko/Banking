using System.Globalization;
using System.Security.Cryptography;

namespace Banking.Domain
{
    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public string HolderName { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();

        public virtual ICollection<Transfer> TransferFrom { get; } = new List<Transfer>();
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

                return string.Join("", randomNumber.Select(b => (b % 10)));
            }
        }
    }
}
