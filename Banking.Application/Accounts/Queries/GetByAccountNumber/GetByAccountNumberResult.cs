using Banking.Domain.Accounts;

namespace Banking.Application.Accounts.Queries.GetByAccountNumber
{
    public record GetByAccountNumberResult(Account? Account);
}
