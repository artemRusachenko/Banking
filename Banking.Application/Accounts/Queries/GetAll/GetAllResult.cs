using Banking.Domain.Accounts;

namespace Banking.Application.Accounts.Queries.GetAll
{
    public record GetAllResult(IEnumerable<Account> Accounts);
}
