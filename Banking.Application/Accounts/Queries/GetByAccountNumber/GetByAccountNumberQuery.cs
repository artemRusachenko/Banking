using Banking.Application.Accounts.Queries.GetByAccountNumber;
using Banking.Application.Core;
using MediatR;

namespace Banking.API.Accounts.Queries.GetByAccountNumber
{
    public record GetByAccountNumberQuery(string AccountNumber) : IRequest<Result<GetByAccountNumberResult>>;
}
