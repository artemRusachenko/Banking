using Banking.Application.Core;
using MediatR;

namespace Banking.Application.Accounts.Commands.CreateAccount
{
    public record CreateAccountCommand(string HolderName) : IRequest<Result<Unit>>;
}
