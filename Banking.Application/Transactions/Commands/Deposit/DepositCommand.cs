using Banking.Application.Core;
using MediatR;

namespace Banking.Application.Transactions.Commands.Deposit
{
    public record DepositCommand(string AccountNumber, decimal Amount) : IRequest<Result<DepositResult>>;
}
