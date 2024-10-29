using Banking.Application.Core;
using MediatR;

namespace Banking.Application.Transactions.Commands.Withdraw
{
    public record WithdrawCommand(string AccountNumber, decimal Amount) : IRequest<Result<WithdrawResult>>;
}
