using MediatR;

namespace Banking.Application.Transactions.Commands.Deposit
{
    public record DepositResult(string TransactionId);
}
