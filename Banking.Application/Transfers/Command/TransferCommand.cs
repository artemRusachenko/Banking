using Banking.Application.Core;
using MediatR;

namespace Banking.Application.Transfers.Command
{
    public record TransferCommand(string FromAccountNumber, string ToAccountNumber, decimal Amount, string? description)
        : IRequest<Result<TransferResult>>;
}
